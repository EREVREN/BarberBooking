import Button from "@/shared/ui/Button";
import Section from "../../shared/ui/Section";

export function CallToAction() {
  return (
    <Section className="py-20 bg-gray-100 text-center">
      <h2 className="text-4xl font-bold mb-6">
        Ready for Your Next Cut?
      </h2>

      <p className="mb-8 text-gray-600">
        Book online or soon directly from social media.
      </p>

      <Button to="/booking" size="lg">
        Book Now
      </Button>
    </Section>
  );
}