export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Meta Momentum Entrypoint",
    alias: "MetaMomentum.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint"),
  }
];
