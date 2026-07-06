interface PageHeaderProps {
  eyebrow: string;
  title: string;
  description: string;
  actionLabel?: string;
}

export function PageHeader({ eyebrow, title, description, actionLabel }: PageHeaderProps) {
  return (
    <section className="page-header">
      <div>
        <p className="eyebrow">{eyebrow}</p>
        <h1>{title}</h1>
        <p>{description}</p>
      </div>
      {actionLabel ? <button className="primary-button" type="button">{actionLabel}</button> : null}
    </section>
  );
}