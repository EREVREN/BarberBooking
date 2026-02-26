

import Section from "../../shared/ui/Section";

export function WhyChooseUs() {
  return (
    <Section className="py-20 bg-gray-900 text-white">
      <div className="container mx-auto px-6">
        <h2 className="text-3xl font-bold mb-10">Why Choose Us</h2>

        <ul className="grid md:grid-cols-3 gap-8">
          <li>✔ Smart scheduling</li>
          <li>✔ Instant confirmations</li>
          <li>✔ AI reminders & follow-ups</li>
        </ul>
      </div>
    </Section>
  );
}