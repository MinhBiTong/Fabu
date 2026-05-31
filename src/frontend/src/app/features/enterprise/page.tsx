import { FeatureLandingPage } from "@/features/value-added/FeatureLandingPage";
import { featurePages } from "@/features/value-added/feature-pages";

export default function EnterprisePage() {
  return <FeatureLandingPage config={featurePages.enterprise} />;
}
