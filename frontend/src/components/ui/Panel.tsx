import type { ReactNode } from 'react';

interface PanelProps {
  title: string;
  eyebrow?: string;
  action?: string;
  children: ReactNode;
  wide?: boolean;
}

export function Panel({ title, eyebrow, action, children, wide }: PanelProps) {
  return (
    <section className={`panel${wide ? ' panel-wide' : ''}`}>
      <div className="panel-header">
        <div>
          {eyebrow ? <p className="panel-eyebrow">{eyebrow}</p> : null}
          <h2>{title}</h2>
        </div>
        {action ? <button className="ghost-button" type="button">{action}</button> : null}
      </div>
      {children}
    </section>
  );
}