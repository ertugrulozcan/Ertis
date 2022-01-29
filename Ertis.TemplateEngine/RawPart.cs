namespace Ertis.TemplateEngine
{
    public class RawPart : ITemplateSegment
    {
        #region Properties

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