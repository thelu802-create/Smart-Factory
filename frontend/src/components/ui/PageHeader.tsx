interface PageHeaderProps {
  eyebrow: string;
  title: string;
  description: string;
  actionLabel?: string;
  onAction?: () => void;
  actionBusy?: boolean;
}

export function PageHeader({ eyebrow, title, description, actionLabel, onAction, actionBusy }: PageHeaderProps) {
  return (
    <section className="page-header">
      <div>
        <p className="eyebrow">{eyebrow}</p>
        <h1>{title}</h1>
        <p>{description}</p>
      </div>
      {actionLabel ? (
        <button className="primary-button" type="button" onClick={onAction} disabled={actionBusy}>
          {actionBusy ? 'Working…' : actionLabel}
        </button>
      ) : null}
    </section>
  );
}
