import { formatCurrency } from "@/lib/utils/format";

type PaymentProps = {
  amount: number | null;
  finalPrice: number;
};

export default function Payment({ amount, finalPrice }: PaymentProps) {
  return (
    <div className="rounded bg-fabu-muted p-4">
      <div className="flex justify-between text-sm text-fabu-gray">
        <span>Amount</span>
        <span>{formatCurrency(amount ?? 0)}</span>
      </div>
      <div className="mt-3 flex justify-between text-lg font-bold text-fabu-ink">
        <span>Total payment</span>
        <span>{formatCurrency(finalPrice)}</span>
      </div>
    </div>
  );
}
