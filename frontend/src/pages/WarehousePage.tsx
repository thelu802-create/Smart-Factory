import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { warehouseItems as mockItems } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function WarehousePage() {
  const { data: warehouseItems, reload } = useApiData(mockItems, factoryApi.getWarehouseItems);
  const businessUnits = new Set(warehouseItems.map((item) => item.bu)).size;
  const wrongPlacements = warehouseItems.filter((item) => item.status === 'Wrong Zone').length;
  const lowStockWarnings = warehouseItems.filter((item) => item.status === 'Low Stock').length;

  const [itemId, setItemId] = useState('');
  const [movementType, setMovementType] = useState<'Import' | 'Export'>('Import');
  const [quantity, setQuantity] = useState(1);
  const [note, setNote] = useState('');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  const selectedId = itemId || warehouseItems[0]?.id || '';

  async function submitMovement() {
    if (!selectedId) return;
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const updated = await factoryApi.moveWarehouseItem(selectedId, movementType, Number(quantity), note || undefined);
      setMessage(`${movementType} ${quantity} - ${updated.itemCode} now has ${updated.quantity} units.`);
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
      <Panel title="Stock movement" eyebrow="Import / Export" wide>
        {error && <p className="form-action-error">{error}</p>}
        {message && <p className="form-action-ok">{message}</p>}
        <div className="move-form">
          <label>Item
            <select className="item-select" value={selectedId} onChange={(event) => setItemId(event.target.value)}>
              {warehouseItems.map((item) => (
                <option key={item.id} value={item.id}>{item.itemCode} - {item.itemName} (qty {item.quantity})</option>
              ))}
            </select>
          </label>
          <label>Type
            <select value={movementType} onChange={(event) => setMovementType(event.target.value as 'Import' | 'Export')}>
              <option value="Import">Import (in)</option>
              <option value="Export">Export (out)</option>
            </select>
          </label>
          <label>Quantity
            <input className="qty" type="number" min={1} value={quantity} onChange={(event) => setQuantity(Number(event.target.value))} />
          </label>
          <label>Note
            <input className="note" type="text" value={note} placeholder="optional" onChange={(event) => setNote(event.target.value)} />
          </label>
          <button className="primary-button" type="button" disabled={busy || !selectedId} onClick={submitMovement}>
            {busy ? 'Working…' : 'Record movement'}
          </button>
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
