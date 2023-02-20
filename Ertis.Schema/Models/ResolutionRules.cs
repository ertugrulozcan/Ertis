using System;
using Newtonsoft.Json;

namespace Ertis.Schema.Models;

public struct ResolutionRules : ICloneable
{
	#region Fields

    private readonly int? maxWidth;
    private readonly int? maxHeight;
    private readonly int? minWidth;
    private readonly int? minHeight;
    
    #endregion
    
	#region Properties

    [JsonProperty("minWidth", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinWidth
    {
        get => this.minWidth;
        init
        {
            this.minWidth = value;
            
            if (!this.ValidateMinWidth(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("minHeight", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinHeight
    {
        get => this.minHeight;
        init
        {
            this.minHeight = value;
            
            if (!this.ValidateMinHeight(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("maxWidth", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxWidth
    {
        get => this.maxWidth;
        init
        {
            this.maxWidth = value;
            
            if (!this.ValidateMaxWidth(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("maxHeight", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxHeight
    {
        get => this.maxHeight;
        init
        {
            this.maxHeight = value;
            
            if (!this.ValidateMaxHeight(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("recommendedWidth", NullValueHandling = NullValueHandling.Ignore)]
    public int? RecommendedWidth { get; set; }
    
    [JsonProperty("recommendedHeight", NullValueHandling = NullValueHandling.Ignore)]
    public int? RecommendedHeight { get; set; }
    
    [JsonProperty("maxSizesRequired")]
    public bool MaxSizesRequired { get; set; }
    
    [JsonProperty("minSizesRequired")]
    public bool MinSizesRequired { get; set; }
    
    [JsonProperty("aspectRatioRequired")]
    public bool AspectRatioRequired { get; set; }

    #endregion
    
    #region Methods

    private bool ValidateMinWidth(out Exception exception)
    {
        if (this.MinWidth != null)
        {
            if (this.MinWidth < 0)
            {
                exception = new Exception($"The 'minWidth' value can not be less than zero')");
                return false;
            }

            if (this.MaxWidth != null && this.MinWidth != null && this.MaxWidth < this.MinWidth)
            {
                exception = new Exception($"The 'minWidth' value can not be greater than the 'maxWidth' value')");
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMinHeight(out Exception exception)
    {
        if (this.MinHeight != null)
        {
            if (this.MinHeight < 0)
            {
                exception = new Exception($"The 'minHeight' value can not be less than zero')");
                return false;
            }

            if (this.MaxHeight != null && this.MinHeight != null && this.MaxHeight < this.MinHeight)
            {
                exception = new Exception($"The 'minHeight' value can not be greater than the 'maxHeight' value')");
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaxWidth(out Exception exception)
    {
        if (this.MaxWidth != null)
        {
            if (this.MaxWidth < 0)
            {
                exception = new Exception($"The 'maxWidth' value can not be less than zero')");
                return false;
            }

            if (this.MinWidth != null && this.MaxWidth != null && this.MinWidth > this.MaxWidth)
            {
                exception = new Exception($"The 'minWidth' value can not be greater than the 'maxWidth' value')");
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaxHeight(out Exception exception)
    {
        if (this.MaxHeight != null)
        {
            if (this.MaxHeight < 0)
            {
                exception = new Exception($"The 'maxHeight' value can not be less than zero')");
                return false;
            }

            if (this.MinHeight != null && this.MaxHeight != null && this.MinHeight > this.MaxHeight)
            {
                exception = new Exception($"The 'minHeight' value can not be greater than the 'maxHeight' value')");
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    public object Clone()
    {
        return new ResolutionRules
        {
            MinWidth = this.MinWidth,
            MinHeight = this.MinHeight,
            MaxWidth = this.MaxWidth,
            MaxHeight = this.MaxHeight,
            RecommendedWidth = this.RecommendedWidth,
            RecommendedHeight = this.RecommendedHeight,
            MinSizesRequired = this.MinSizesRequired,
            MaxSizesRequired = this.MaxSizesRequired,
            AspectRatioRequired = this.AspectRatioRequired
        };
    }

    #endregion
}