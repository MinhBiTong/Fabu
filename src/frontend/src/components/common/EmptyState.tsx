type EmptyStateProps = {
  title: string;
  description?: string;
};

export function EmptyState({ title, description }: EmptyStateProps) {
  return (
    <div className="rounded-card border border-dashed border-fabu-border bg-white p-8 text-center">
      <h3 className="text-xl">{title}</h3>
      {description ? <p className="mt-2 text-sm text-fabu-gray">{description}</p> : null}
    </div>
  );
}
