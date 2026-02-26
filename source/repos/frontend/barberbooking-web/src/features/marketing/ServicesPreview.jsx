import Button from "@/shared/ui/Button";
import serviceImg1 from "@/assets/img/service-haircut.jpg";
import serviceImg2 from "@/assets/img/service-beard.jpg";
import serviceImg3 from "@/assets/img/service-hair-beard.jpg";
import Section from "../../shared/ui/Section";

const services = [
  { name: "Haircut", duration: "30 min" },
  { name: "Beard Trim", duration: "20 min" },
  { name: "Hair + Beard", duration: "45 min" },
];
const serviceImages = [serviceImg1, serviceImg2, serviceImg3];


export function ServicesPreview() {
  return (
    <Section className="py-20 bg-gray-100">
      <div className="container mx-auto px-6">
        <h2 className="text-3xl font-bold mb-10">Our Services</h2>

        <div className="grid md:grid-cols-3 gap-6 hover:shadow-lg transition">
          {services.map((service, index)=> (
            <div
              key={service.name}
              className="bg-white p-6 rounded-xl shadow hover:shadow-lg transition"
            >
              <div className="w-96 h-84 sm:w-36 sm:h-36 md:w-64 md:h-64 mb-4 flex-shrink-0">
                        <img 
                          src={serviceImages[index % serviceImages.length]} 
                          alt={service.name} 
                          className="rounded-2xl object-cover w-full h-full"
                        />
                      </div>
              <h3 className="text-xl font-semibold">{service.name}</h3>
              <p className="text-gray-500">{service.duration}</p>
            </div>
          ))}
        </div>

        <div className="mt-10">
          <Button to="/services">See All Services</Button>
        </div>
      </div>
    </Section>
  );
}