using RoR2;
using RoR2.ContentManagement;
using System.Collections;

namespace GeneticsArtifact
{
    internal class GeneticsContentProvider : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => GeneticsArtifactPlugin.ModGuid;

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            args.output.artifactDefs.Add(new ArtifactDef[] { ArtifactOfGenetics.def });
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
