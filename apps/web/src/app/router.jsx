
import { createBrowserRouter } from "react-router-dom";
import Home from "@/pages/Home";
import BookingPage from "@/features/booking/pages/BookingPage";
import Services from "@/pages/Services";
import Barbers from "@/pages/Barbers";
import About from "@/pages/About";
import Contact from "@/pages/Contact";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Home />
  },
  {
    path: "/booking",
    element: <BookingPage />
  },
  {
    path: "/services",
    element: <Services />
  },
  {
    path: "/barbers",
    element: <Barbers />
  },
  {
    path: "/about",
    element: <About />
  },
  {
    path: "/contact",
    element: <Contact />
  }
]);
