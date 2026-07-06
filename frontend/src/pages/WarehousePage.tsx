import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { warehouseItems as mockItems } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function WarehousePage() {
  const { data: warehouseItems } = useApiData(mockItems, factoryApi.getWarehouseItems);
  const businessUnits = new Set(warehouseItems.map((item) => item.bu)).size;
  const wrongPlacements = warehouseItems.filter((item) => item.status === 'Wrong Zone').length;
  const lowStockWarnings = warehouseItems.filter((item) => item.status === 'Low Stock').length;

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Warehouse intelligence" title="Warehouse Management" description="Search item locations, review warehouse capacity, resolve wrong placement alerts, and accept AI storage recommendations." actionLabel="New movement" />
      <section className="summary-strip">
        <div><span>Business units</span><strong>{businessUnits}</strong></div>
        <div><span>Wrong placements</span><strong>{wrongPlacements}</strong></div>
        <div><span>Low stock warnings</span><strong>{lowStockWarnings}</strong></div>
      </section>
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