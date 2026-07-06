interface StatusBadgeProps {
  value: string;
}

export function StatusBadge({ value }: StatusBadgeProps) {
  const tone = value.toLowerCase().replace(/\s+/g, '-');
  return <span className={`status-badge ${tone}`}>{value}</span>;
}