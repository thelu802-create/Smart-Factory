import { useEffect, useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { warehouseItems as mockItems } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';
import type { GoodsMovement, WarehouseZone } from '../types';

type MovementType = 'Import' | 'Export' | 'Transfer';

export function WarehousePage() {
  const { data: warehouseItems, reload } = useApiData(mockItems, factoryApi.getWarehouseItems);
  const { data: zones } = useApiData<WarehouseZone[]>([], factoryApi.getWarehouseZones);
  const businessUnits = new Set(warehouseItems.map((item) => item.bu)).size;
  const wrongPlacements = warehouseItems.filter((item) => item.status === 'Wrong Zone').length;
  const lowStockWarnings = warehouseItems.filter((item) => item.status === 'Low Stock').length;

  const [itemId, setItemId] = useState('');
  const [itemQuery, setItemQuery] = useState('');
  const [movementType, setMovementType] = useState<MovementType>('Import');
  const [quantity, setQuantity] = useState(1);
  const [toZoneId, setToZoneId] = useState('');
  const [note, setNote] = useState('');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [history, setHistory] = useState<GoodsMovement[]>([]);

  const query = itemQuery.trim().toLowerCase();
  const filteredItems = query
    ? warehouseItems.filter((item) =>
        [item.itemCode, item.itemName, item.batchCode, item.bu, item.ioId, item.ioCode]
          .some((field) => field.toLowerCase().includes(query)))
    : warehouseItems;
  const selectedId = itemId && filteredItems.some((item) => item.id === itemId)
    ? itemId
    : filteredItems[0]?.id ?? '';
  const selectedItem = warehouseItems.find((item) => item.id === selectedId);
  const targetZones = zones.filter((zone) => zone.name !== selectedItem?.zone);

  useEffect(() => {
    if (!selectedId) return;
    let active = true;
    factoryApi.getItemMovements(selectedId)
      .then((data) => { if (active) setHistory(data); })
      .catch(() => { if (active) setHistory([]); });
    return () => { active = false; };
  }, [selectedId, message]);

  async function submitMovement() {
    if (!selectedId) return;
    if (movementType === 'Transfer' && !toZoneId) {
      setError('Select a target zone for the transfer.');
      return;
    }
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const updated = await factoryApi.moveWarehouseItem(
        selectedId,
        movementType,
        Number(quantity),
        movementType === 'Transfer' ? toZoneId : undefined,
        note || undefined,
      );
      setMessage(`${movementType} - ${updated.itemCode}: now ${updated.quantity} units in ${updated.zone} (${updated.status}).`);
      setNote('');
      reload();
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : 'Movement failed');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Warehouse intelligence" title="Warehouse Management" description="Search item locations, review warehouse capacity, resolve wrong placement alerts, and accept AI storage recommendations." />
      <section className="summary-strip">
        <div><span>Business units</span><strong>{businessUnits}</strong></div>
        <div><span>Wrong placements</span><strong>{wrongPlacements}</strong></div>
        <div><span>Low stock warnings</span><strong>{lowStockWarnings}</strong></div>
      </section>
      <Panel title="Stock movement" eyebrow="Import / Export / Transfer" wide>
        {error && <p className="form-action-error">{error}</p>}
        {message && <p className="form-action-ok">{message}</p>}
        <div className="move-form">
          <label>Find item
            <input className="item-search" type="text" value={itemQuery} placeholder="code, name, batch, BU, IO…" onChange={(event) => setItemQuery(event.target.value)} />
          </label>
          <label>Item{query ? ` (${filteredItems.length} match)` : ''}
            <select className="item-select" value={selectedId} onChange={(event) => setItemId(event.target.value)} disabled={filteredItems.length === 0}>
              {filteredItems.length === 0 ? (
                <option value="">No matching item</option>
              ) : (
                filteredItems.map((item) => (
                  <option key={item.id} value={item.id}>{item.itemCode} - {item.itemName} (qty {item.quantity}, {item.zone})</option>
                ))
              )}
            </select>
          </label>
          <label>Type
            <select value={movementType} onChange={(event) => setMovementType(event.target.value as MovementType)}>
              <option value="Import">Import (in)</option>
              <option value="Export">Export (out)</option>
              <option value="Transfer">Transfer (zone)</option>
            </select>
          </label>
          {movementType === 'Transfer' ? (
            <label>To zone
              <select value={toZoneId} onChange={(event) => setToZoneId(event.target.value)}>
                <option value="">Select zone…</option>
                {targetZones.map((zone) => (
                  <option key={zone.id} value={zone.id}>{zone.name} ({zone.currentUsage}/{zone.capacity})</option>
                ))}
              </select>
            </label>
          ) : (
            <label>Quantity
              <input className="qty" type="number" min={1} value={quantity} onChange={(event) => setQuantity(Number(event.target.value))} />
            </label>
          )}
          <label>Note
            <input className="note" type="text" value={note} placeholder="optional" onChange={(event) => setNote(event.target.value)} />
          </label>
          <button className="primary-button" type="button" disabled={busy || !selectedId} onClick={submitMovement}>
            {busy ? 'Working…' : 'Record movement'}
          </button>
        </div>
        <div className="movement-history">
          <p className="eyebrow">Movement history{selectedItem ? ` - ${selectedItem.itemCode}` : ''}</p>
          {history.length === 0 ? (
            <p className="movement-empty">No movements recorded for this item yet.</p>
          ) : (
            history.map((move) => (
              <div className="movement-row" key={move.id}>
                <span><strong>{move.movementType}</strong> {move.quantity}</span>
                <span>{move.fromZone ? `${move.fromZone} → ` : ''}{move.toZone}</span>
                <span>{move.movedAt}</span>
                <small>{move.note}</small>
              </div>
            ))
          )}
        </div>
      </Panel>
      <Panel title="Item location tracking" eyebrow="Inventory" action="Search item" wide>
        <div className="data-table">
          <div className="table-head warehouse-grid"><span>BU / IO</span><span>Item</span><span>Zone</span><span>Qty</span><span>Status</span><span>Last movement</span></div>
          {warehouseItems.map((item) => (
            <div className="table-row warehouse-grid" key={item.id}>
              <span><strong>{item.bu}</strong><small>{item.ioId} - {item.ioCode}</small></span>
              <span><strong>{item.itemCode}</strong><small>{item.itemName} - {item.batchCode}</small></span>
              <span>{item.zone}<small>{item.shelf}</small></span>
              <strong>{item.quantity}</strong>
              <StatusBadge value={item.status} />
              <span>{item.lastMovementAt}</span>
            </div>
          ))}
        </div>
      </Panel>
    </div>
  );
}
