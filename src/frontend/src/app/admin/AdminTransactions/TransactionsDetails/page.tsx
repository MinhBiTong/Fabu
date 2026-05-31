const details = [
  ["Transaction Date", "2026-05-31"],
  ["Customer Name", "Fabu Customer"],
  ["Phone Number", "0912345678"],
  ["Transaction Status", "Success"],
  ["Service Type", "Recharge"],
  ["Service Name", "Mobile Recharge"],
  ["Payment Type", "VNPay"],
  ["Price", "100,000 VND"],
];

export default function TransactionsDetailsPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <h1>Transaction Details</h1>
        <div className="fabu-card divide-y divide-fabu-border p-0">
          {details.map(([label, value]) => (
            <div key={label} className="grid gap-2 p-4 sm:grid-cols-2">
              <span className="text-sm text-fabu-gray">{label}</span>
              <span className="font-semibold text-fabu-ink">{value}</span>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
