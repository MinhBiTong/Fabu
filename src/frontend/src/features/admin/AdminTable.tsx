import type { ReactNode } from "react";

type AdminTableProps = {
  headers: string[];
  children: ReactNode;
  empty?: ReactNode;
  isEmpty?: boolean;
};

export function AdminTable({ headers, children, empty, isEmpty }: AdminTableProps) {
  return (
    <div className="overflow-hidden rounded-card border border-fabu-border bg-white shadow-elevated">
      <div className="overflow-x-auto">
        <table className="w-full min-w-[720px] border-collapse text-sm">
          <thead className="bg-fabu-muted text-left text-fabu-gray">
            <tr>
              {headers.map((header) => (
                <th key={header} className="p-4 font-semibold">
                  {header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {isEmpty ? (
              <tr>
                <td className="p-6 text-center text-fabu-gray" colSpan={headers.length}>
                  {empty ?? "No data"}
                </td>
              </tr>
            ) : (
              children
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
