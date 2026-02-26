import { useState, useCallback } from "react";
import {ToastContext} from "./ToastContext";
import Toast from "../ui/Toast";

let idCounter = 0;
const containerStyle = {
  position: "fixed",
  top: 16,
  right: 16,
  display: "flex",
  flexDirection: "column",
  gap: 12,
  zIndex: 9999
};
export default function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);

  const showToast = useCallback((message, type = "info", duration = 3000) => {
    const id = ++idCounter;

    setToasts(prev => [...prev, { id, message, type }]);

    setTimeout(() => {
      setToasts(prev => prev.filter(t => t.id !== id));
    }, duration);
  }, []);

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}

      <div style={containerStyle}>
        {toasts.map(t => (
          <Toast key={t.id} {...t} />
        ))}
      </div>
    </ToastContext.Provider>
  );
}

