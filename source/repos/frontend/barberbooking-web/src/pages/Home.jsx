import { HeroSection } from "@/features/marketing/HeroSection";
import { ServicesPreview } from "@/features/marketing/ServicesPreview";
import { BarbersPreview } from "@/features/marketing/BarbersPreview";
import { WhyChooseUs } from "@/features/marketing/WhyChooseUs";
import { CallToAction } from "@/features/marketing/CallToAction";
import { MainLayout } from "@/shared/layout/MainLayout";

export default function Home() {
  return (
    <MainLayout>
      <HeroSection />
      <ServicesPreview />
      <BarbersPreview />
      <WhyChooseUs />
      <CallToAction />
    </MainLayout>
  );
}
