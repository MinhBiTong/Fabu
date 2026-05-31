import { FeatureLandingPage } from "@/features/value-added/FeatureLandingPage";
import { featurePages } from "@/features/value-added/feature-pages";

export default function SimStorePage() {
  return <FeatureLandingPage config={featurePages.simStore} />;
}
