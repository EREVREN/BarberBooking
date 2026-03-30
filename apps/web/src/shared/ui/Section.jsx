export default function Section({
  children,
  className = "",
  id,
}) {
  return (
    <section
      id={id}
      className={`max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20 ${className}`}
    >
      {children}
    </section>
  );
}