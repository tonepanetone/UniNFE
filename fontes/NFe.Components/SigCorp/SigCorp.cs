﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFe.Components.SigCorp
{
    public class SigCorp : SigCorpBase
    {
        #region Construtures
        public SigCorp(TipoAmbiente tpAmb, string pastaRetorno, int codMun)
            : base(tpAmb, pastaRetorno, codMun)
        { }
        #endregion
    }
}
