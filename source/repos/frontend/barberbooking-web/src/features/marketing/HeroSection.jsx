import Button from "@/shared/ui/Button";
import Section from "@/shared/ui/Section";
import barberShopImg from "@/assets/img/service-allServices.jpg";

export function HeroSection() {
  return (
    <Section className="bg-black text-white min-h-[80vh] flex items-center justify-center">
     <Section className="flex flex-col md:flex-row items-center gap-10">
      <div className="container mx-auto px-6">
        <h1 className="text-5xl font-bold mb-6">
          Premium Barber Experience
          
        </h1>
        <p className="text-lg text-gray-300 max-w-xl mb-8">
          Book your barber in seconds. Smart scheduling, instant confirmation,
          and AI-powered reminders.
        </p>

        <div className="flex gap-4">
          <Button to="/booking" variant="primary">
            Book Appointment
          </Button>

          <Button to="/services" variant="outline">
            View Services
          </Button>
        </div>

        {/* AI future hook */}
        <p className="mt-6 text-sm text-gray-400">
          Soon available via WhatsApp & Instagram DM 🤖
        </p>
      </div>
      </Section>
       <div className="w-full md:w-1/2">
         <img
           src={barberShopImg}
           alt="Barber Shop"
           className="w-full h-auto rounded-lg shadow-lg object-cover"
         />
       </div>
    </Section>
  );
}
