import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/Button";

export function Navbar() {
  const [isOpen, setIsOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  function handleBookNow() {
    setIsOpen(false);
    navigate("/booking", { state: { resetAt: Date.now() } });
  }

  return (
    <nav className="bg-[#0f0b07] text-[#f5ede3] border-b border-[#1f1a14] shadow-[0_10px_30px_rgba(0,0,0,0.45)]">
      <div className="container mx-auto px-6 py-5 flex items-center justify-between">
        
        {/* Logo */}
        <Link
          to="/"
          className="text-2xl font-serif uppercase tracking-[0.25em] text-[#cd905b] border border-[#cd905b] px-3 py-1 hover:bg-[#cd905b] hover:text-black transition"
        >
          BarberBooking
        </Link>

        {/* Navigation */}
        <div className="desktop-nav gap-4 items-center text-xs uppercase tracking-[0.2em]">
          <Link
            to="/"
            className="px-3 py-2 border border-transparent rounded-md hover:border-[#cd905b] hover:text-[#cd905b] transition"
          >
            Home
          </Link>
          <Link
            to="/services"
            className="px-3 py-2 border border-transparent rounded-md hover:border-[#cd905b] hover:text-[#cd905b] transition"
          >
            Services
          </Link>

          <Link
            to="/barbers"
            className="px-3 py-2 border border-transparent rounded-md hover:border-[#cd905b] hover:text-[#cd905b] transition"
          >
            Barbers
          </Link>

          <Link
            to="/about"
            className="px-3 py-2 border border-transparent rounded-md hover:border-[#cd905b] hover:text-[#cd905b] transition"
          >
            About
          </Link>

          <Link
            to="/contact"
            className="px-3 py-2 border border-transparent rounded-md hover:border-[#cd905b] hover:text-[#cd905b] transition"
          >
            Contact
          </Link>
        </div>

        {/* Actions */}
        <div className="flex gap-3 items-center">
          {/* AI hook */}
          <span className="hidden md:block text-xs text-[#a89a8a] uppercase tracking-[0.18em]">
            Book via WhatsApp 🤖
          </span>

          <Button
            onClick={handleBookNow}
            size="sm"
            variant="outline"
            className="border-[#cd905b] text-[#cd905b] bg-transparent hover:bg-[#cd905b] hover:text-black"
          >
            Book Now
          </Button>

          <button
            type="button"
            aria-label="Toggle navigation menu"
            aria-expanded={isOpen}
            onClick={() => setIsOpen((prev) => !prev)}
            className="items-center justify-center rounded-md p-2 text-[#cd905b] border border-[#2a1f16] hover:bg-[#cd905b] hover:text-black transition"
          >
            <svg
              className="h-6 w-6"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              {isOpen ? (
                <>
                  <line x1="18" y1="6" x2="6" y2="18" />
                  <line x1="6" y1="6" x2="18" y2="18" />
                </>
              ) : (
                <>
                  <line x1="3" y1="6" x2="21" y2="6" />
                  <line x1="3" y1="12" x2="21" y2="12" />
                  <line x1="3" y1="18" x2="21" y2="18" />
                </>
              )}
            </svg>
          </button>
        </div>
      </div>

      <div
        className={[
          "border-t border-[#1f1a14] bg-[#0f0b07] overflow-hidden transition-all duration-300 ease-out",
          isOpen ? "max-h-64 opacity-100" : "max-h-0 opacity-0",
        ].join(" ")}
      >
        <div className="container mx-auto px-6 py-5 flex flex-col gap-4 text-xs uppercase tracking-[0.2em]">
          <Link to="/" className="hover:text-[#cd905b] transition" onClick={() => setIsOpen(false)}>
            Home
          </Link>
          <Link to="/services" className="hover:text-[#cd905b] transition" onClick={() => setIsOpen(false)}>
            Services
          </Link>
          <Link to="/barbers" className="hover:text-[#cd905b] transition" onClick={() => setIsOpen(false)}>
            Barbers
          </Link>
          <Link to="/about" className="hover:text-[#cd905b] transition" onClick={() => setIsOpen(false)}>
            About
          </Link>
          <Link to="/contact" className="hover:text-[#cd905b] transition" onClick={() => setIsOpen(false)}>
            Contact
          </Link>
        </div>
      </div>
    </nav>
  );
}
