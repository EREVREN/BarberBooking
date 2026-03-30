import React from "react";
import ReactDOM from "react-dom/client";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "./app/queryClient";
import ToastProvider from "./shared/toast/ToastProvider";
import App from "./app/App";
import "./index.css";
import "./App.css";


ReactDOM.createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <QueryClientProvider client={queryClient}>
             <ToastProvider>
                <App />
             </ToastProvider>                 
        </QueryClientProvider>
    </React.StrictMode>
);



