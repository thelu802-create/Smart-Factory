const kpis = [
  {
    title: "Daily output",
    value: "12,480",
    note: "Units completed today",
    trend: "+6.4%",
    tone: "good"
  },
  {
    title: "Active lines",
    value: "18/20",
    note: "2 lines under maintenance",
    trend: "Stable",
    tone: "good"
  },
  {
    title: "Safety alerts",
    value: "07",
    note: "1 critical alert open",
    trend: "Review now",
    tone: "bad"
  },
  {
    title: "Warehouse occupancy",
    value: "82%",
    note: "Zone C nearing full capacity",
    trend: "Watch",
    tone: "warn"
  }
];

const lines = [
  { name: "Line A", detail: "Packaging and labeling", status: "running", metric: "97%" },
  { name: "Line B", detail: "Assembly sequence stable", status: "running", metric: "94%" },
  { name: "Line C", detail: "Speed below target due to staffing", status: "warning", metric: "81%" },
  { name: "Line D", detail: "Sensor recalibration in progress", status: "alert", metric: "Stopped" }
];

const alerts = [
  { title: "Restricted zone entry", description: "One operator detected near Robot Cell 2 without clearance.", level: "Critical" },
  { title: "Forklift congestion", description: "Traffic density increased in Warehouse Zone C during inbound peak.", level: "Medium" },
  { title: "Temperature variance", description: "Storage room B exceeded safe threshold for 6 minutes.", level: "Medium" }
];

const warehouse = [
  { title: "Zone C capacity", description: "Current usage reached 92% of approved storage threshold." },
  { title: "Misplaced pallet", description: "Item batch RM-204 stored in finished goods aisle." },
  { title: "Low stock warning", description: "Fastener kit FK-18 will run out within the next 9 hours." }
];

const approvals = [
  { title: "Overtime request", description: "Night shift for Line 3 awaiting manager approval." },
  { title: "Machine fault report", description: "Press unit P-14 submitted with urgent maintenance note." },
  { title: "Warehouse export form", description: "Dispatch request for Batch FG-887 pending release." }
];

const plannerSuggestions = [
  "Move 2 certified operators from Line 5 to Line 3 for the night shift.",
  "Approve 1.5 hours of overtime for the packaging team to recover 320 units.",
  "Delay non-urgent maintenance on Line B until after the afternoon target is met."
];

function renderKpis() {
  const container = document.getElementById("kpi-grid");
  container.innerHTML = kpis.map((item) => `
    <article class="kpi-card">
      <div class="kpi-top">
        <span class="kpi-title">${item.title}</span>
        <span class="kpi-pill ${item.tone}">${item.trend}</span>
      </div>
      <div class="kpi-value">${item.value}</div>
      <div class="kpi-bottom">
        <span>${item.note}</span>
      </div>
    </article>
  `).join("");
}

function renderLines() {
  const container = document.getElementById("line-list");
  container.innerHTML = lines.map((line) => `
    <div class="line-row">
      <div>
        <strong>${line.name}</strong>
        <span>${line.detail}</span>
      </div>
      <span class="status-chip ${line.status}">${line.status}</span>
      <strong class="metric-inline">${line.metric}</strong>
    </div>
  `).join("");
}

function renderAlerts() {
  const container = document.getElementById("alert-list");
  container.innerHTML = alerts.map((alert) => `
    <div class="alert-row">
      <strong>${alert.title}</strong>
      <span>${alert.description}</span>
      <span class="status-chip ${alert.level === "Critical" ? "alert" : "warning"}">${alert.level}</span>
    </div>
  `).join("");
}

function renderWarehouse() {
  const container = document.getElementById("warehouse-list");
  container.innerHTML = warehouse.map((item) => `
    <div class="warehouse-row">
      <strong>${item.title}</strong>
      <span>${item.description}</span>
    </div>
  `).join("");
}

function renderApprovals() {
  const container = document.getElementById("approval-list");
  container.innerHTML = approvals.map((item) => `
    <div class="approval-row">
      <strong>${item.title}</strong>
      <span>${item.description}</span>
    </div>
  `).join("");
}

function renderPlanner() {
  const container = document.getElementById("planner-suggestions");
  container.innerHTML = plannerSuggestions.map((item) => `<li>${item}</li>`).join("");
}

renderKpis();
renderLines();
renderAlerts();
renderWarehouse();
renderApprovals();
renderPlanner();