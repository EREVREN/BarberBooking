import { Link } from "react-router-dom";

export function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-300">
      <div className="container mx-auto px-6 py-12 grid md:grid-cols-3 gap-8">

        {/* Brand */}
        <div>
          <h3 className="text-xl font-bold text-white mb-4">
            BarberBooking
          </h3>
          <p className="text-sm">
            Smart barber appointments with AI-powered scheduling.
          </p>
        </div>

        {/* Links */}
        <div>
          <h4 className="font-semibold mb-3 text-white">Quick Links</h4>
          <ul className="space-y-2">
            <li><Link to="/services">Services</Link></li>
            <li><Link to="/barbers">Barbers</Link></li>
            <li><Link to="/booking">Book Appointment</Link></li>
          </ul>
        </div>

        {/* AI / Contact */}
        <div>
          <h4 className="font-semibold mb-3 text-white">Contact</h4>
          <p className="text-sm">📍 Istanbul, Türkiye</p>
          <p className="text-sm">📞 +90 555 555 55 55</p>
          <p className="text-sm mt-2 text-green-400">
            WhatsApp Booking (AI)
          </p>
        </div>

      </div>

      <div className="text-center text-sm py-4 border-t border-gray-700">
        © {new Date().getFullYear()} BarberBooking. All rights reserved.
      </div>
    </footer>
  );
}