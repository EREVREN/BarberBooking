

export default function Toast({ message, type }) {
  const colors = {
    success: "#16a34a",
    error: "#dc2626",
    info: "#2563eb",
    warning: "#ca8a04"
  };

  return (
    <div
      style={{
        minWidth: 240,
        padding: "12px 16px",
        borderRadius: 10,
        background: colors[type] || "#333",
        color: "#fff",
        boxShadow: "0 8px 20px rgba(0,0,0,0.15)",
        fontSize: 14
      }}
    >
      {message}
    </div>
  );
}





