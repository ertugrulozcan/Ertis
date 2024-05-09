namespace Ertis.TemplateEngine
{
    public class RawPart : ITemplateSegment
    {
        #region Properties
        
        public SegmentType Type => SegmentType.RawPart;

        public string RawValue { get; init; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this.RawValue;
        }

        #endregion
    }
}