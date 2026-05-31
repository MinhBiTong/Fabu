type LoadingStateProps = {
  label?: string;
};

export function LoadingState({ label = "Loading..." }: LoadingStateProps) {
  return (
    <div className="flex min-h-48 items-center justify-center text-sm text-fabu-gray">
      {label}
    </div>
  );
}
