﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFe.Components;

namespace NFSe.Components
{
    public class SchemaXMLNFSe_GIF
    {
        public static void CriarListaIDXML()
        {
            #region Schemas GIF - Específico município de Parobé

            #region XML de Consulta de NFSe por Rps
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-NFSE", new InfSchema()
            {
                Tag = "NFSE",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de Consulta de NFSe por Data",
                TagAssinatura = "pedidoLoteNFSe",
                TagAtributoId = "pedidoLoteNFSe",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Consulta de NFSe por Data
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-pedidoLoteNFSe", new InfSchema()
            {
                Tag = "pedidoLoteNFSe",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de NFSe padrão GIF",
                TagAssinatura = "",
                TagAtributoId = "",
                TargetNameSpace = ""
            });
            #endregion

            #region XML de Consulta de NFSe por Rps
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-pedConsultaTrans", new InfSchema()
            {
                Tag = "pedConsultaTrans",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de Consulta de NFSe por Rps",
                TagAssinatura = "pedConsultaTrans",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Consulta de Lote RPS
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-pedidoNFSe", new InfSchema()
            {
                Tag = "pedidoNFSe",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de Consulta de Lote RPS",
                TagAssinatura = "pedidoNFSe",
                TagAtributoId = "pedidoNFSe",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Cancelamento de NFS-e
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-pedidoCancelamentoLote", new InfSchema()
            {
                Tag = "pedidoCancelamentoLote",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de Cancelamento da NFS-e",
                TagAssinatura = "pedidoCancelamentoLote",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Consulta Situação do Lote RPS
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-pedidoStatusLote", new InfSchema()
            {
                Tag = "pedidoStatusLote",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de Consulta da Situacao do Lote RPS",
                TagAssinatura = "pedidoStatusLote",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de lote RPS
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-envioLote", new InfSchema()
            {
                Tag = "envioLote",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Lote RPS",
                TagAssinatura = "envioLote",
                TagAtributoId = "NFS-e",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de consulta URL NFSe
            SchemaXML.InfSchemas.Add("NFSE-GIF-4314050-pedidoNFSePNG", new InfSchema()
            {
                Tag = "pedidoNFSePNG",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\nfse-parobe-v1-1.xsd",
                Descricao = "XML de consulta URL NFe padrão GIF",
                TagAssinatura = "pedidoLoteNFSePNG",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #endregion

            #region Schemas GIF - Demais municípios

            #region XML de Consulta de NFSe por Rps
            SchemaXML.InfSchemas.Add("NFSE-GIF-NFSE", new InfSchema()
            {
                Tag = "NFSE",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Consulta de NFSe por Data",
                TagAssinatura = "pedidoLoteNFSe",
                TagAtributoId = "pedidoLoteNFSe",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Consulta de NFSe por Data
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedidoLoteNFSe", new InfSchema()
            {
                Tag = "pedidoLoteNFSe",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "",
                Descricao = "XML de NFSe padrão GIF",
                TagAssinatura = "",
                TagAtributoId = "",
                TargetNameSpace = ""
            });
            #endregion

            #region XML de Consulta de NFSe por Rps
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedConsultaTrans", new InfSchema()
            {
                Tag = "pedConsultaTrans",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Consulta de NFSe por Rps",
                TagAssinatura = "pedConsultaTrans",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Consulta de Lote RPS / Obter XML da NFSe para o padrão GIF
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedidoNFSe", new InfSchema()
            {
                Tag = "pedidoNFSe",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Consulta de Lote RPS / Obter XML da NFSe para o padrão GIF",
                TagAssinatura = "pedidoNFSe",
                TagAtributoId = "pedidoNFSe",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Cancelamento de NFS-e
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedAnulaNFSe", new InfSchema()
            {
                Tag = "pedAnulaNFSe",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Cancelamento da NFS-e",
                TagAssinatura = "pedAnulaNFSe",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });

            SchemaXML.InfSchemas.Add("NFSE-GIF-pedCancelaNFSe", new InfSchema()
            {
                Tag = "pedCancelaNFSe",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Cancelamento da NFS-e",
                TagAssinatura = "pedCancelaNFSe",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Consulta Situação do Lote RPS
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedidoStatusLote", new InfSchema()
            {
                Tag = "pedidoStatusLote",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Consulta da Situacao do Lote RPS",
                TagAssinatura = "pedidoStatusLote",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de lote RPS
            SchemaXML.InfSchemas.Add("NFSE-GIF-envioLote", new InfSchema()
            {
                Tag = "envioLote",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Lote RPS",
                TagAssinatura = "envioLote",
                TagAtributoId = "NFS-e",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de consulta NFSe em PNG
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedidoNFSePNG", new InfSchema()
            {
                Tag = "pedidoNFSePNG",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de consulta da NFSe em PNG padrão GIF",
                TagAssinatura = "pedidoNFSePNG",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de Inutilização da NFSe
            SchemaXML.InfSchemas.Add("NFSE-GIF-solicitacaoInutilizacao", new InfSchema()
            {
                Tag = "solicitacaoInutilizacao",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de Inutilização da NFSe com padrão GIF/Infisc.",
                TagAssinatura = "solicitacaoInutilizacao",
                TagAtributoId = "numerosInutilizados",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion

            #region XML de consulta NFSe em PNG
            SchemaXML.InfSchemas.Add("NFSE-GIF-pedidoNFSePDF", new InfSchema()
            {
                Tag = "pedidoNFSePDF",
                ID = SchemaXML.InfSchemas.Count + 1,
                ArquivoXSD = "NFSe\\GIF\\NFSe-Infisc-v1.xsd",
                Descricao = "XML de consulta da NFSe em PDF padrão GIF",
                TagAssinatura = "pedidoNFSePDF",
                TagAtributoId = "CNPJ",
                TargetNameSpace = "http://ws.pc.gif.com.br/"
            });
            #endregion            
            
            #endregion
        }
    }
}
