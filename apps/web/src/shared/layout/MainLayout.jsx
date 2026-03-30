import { Navbar } from "@/shared/components/Navbar";
import { Footer } from "@/shared/components/Footer";

export function MainLayout({ children }) {
  return (
    <div className="flex flex-col min-h-screen">
      <Navbar />
      <main className="flex-1">{children}</main>
      <Footer />
    </div>
  );
}
