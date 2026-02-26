import Section from "@/shared/ui/Section";
import { MainLayout } from "@/shared/layout/MainLayout";

export default function Contact() {
  return (
    <MainLayout>
      <Section className="py-20">
        <div>
          <h4 className="font-semibold mb-3 text-white">Contact</h4>
          <p className="text-sm">Istanbul, Turkey</p>
          <p className="text-sm">+90 555 555 55 55</p>
          <p className="text-sm mt-2 text-green-400">
            WhatsApp Booking (AI)
          </p>
        </div>
      </Section>
    </MainLayout>
  );
}
