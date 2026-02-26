import Button from "@/shared/ui/Button";
import { BarberList } from "../barber/BarberList";
import Section from "../../shared/ui/Section";

export function BarbersPreview() {
  return (
    <Section className="py-20 bg-gray-100">
      <div className="container mx-auto px-6">
        <h2 className="text-3xl font-bold mb-10">Meet Our Barbers</h2>

        <BarberList />

        <div className="mt-10">
          <Button to="/barbers">Choose Your Barber</Button>
        </div>
      </div>
    </Section>
  );
}

