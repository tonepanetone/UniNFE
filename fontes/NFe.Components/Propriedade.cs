﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NFe.Components
{
    /// <summary>
    /// Propriedades publicas
    /// </summary>
    public class Propriedade
    {
        public static Assembly AssemblyEXE;

        public static bool ExecutandoPeloUniNFe = true;

        public static string ServiceName = "UniNFeServico";

        /// <summary>
        /// Nome para a pasta dos XML assinados
        /// </summary>
        public const string NomePastaXMLAssinado = "\\Assinado";
        public const string NomeArqERRUniNFe = "UniNFeErro_{0}.err";
        /// <summary>
        /// Nome do arquivo XML de configurações
        /// </summary>
        public const string NomeArqConfig = "UniNfeConfig.xml";
        /// <summary>
        /// Nome do arquivo XML de configurações da tela de sobre
        /// </summary>
        public const string NomeArqConfigSobre = "UniNfeSobre.xml";
        /// <summary>
        /// Nome do arquivo XML que é gravado as empresas cadastradas
        /// </summary>
        public static string NomeArqEmpresas { get { return Propriedade.PastaExecutavel + "\\UniNfeEmpresa.xml"; } }
        /// <summary>     
        /// Nome do arquivo para controle da numeração sequencial do lote.
        /// </summary>
        public const string NomeArqXmlLote = "UniNfeLote.xml";
        /// <summary>
        /// Nome do arquivo 1 de backup de segurança do arquivo de controle da numeração sequencial do lote
        /// </summary>
        public const string NomeArqXmlLoteBkp1 = "Bkp1_UniNfeLote.xml";
        /// <summary>
        /// Nome do arquivo 2 de backup de segurança do arquivo de controle da numeração sequencial do lote
        /// </summary>
        public const string NomeArqXmlLoteBkp2 = "Bkp2_UniNfeLote.xml";
        /// <summary>
        /// Nome do arquivo 3 de backup de segurança do arquivo de controle da numeração sequencial do lote
        /// </summary>
        public const string NomeArqXmlLoteBkp3 = "Bkp3_UniNfeLote.xml";
        /// <summary>
        /// Nome do arquivo que grava as notas fiscais em fluxo de envio
        /// </summary>
        public const string NomeArqXmlFluxoNfe = "fluxonfe.xml";

        /// <summary>
        /// Retorna o nome do XML dos municipios
        /// </summary>
        public static string NomeArqXMLMunicipios
        {
            get { return Propriedade.PastaExecutavel + "\\UniNFeMunic.xml"; }
        }

        /// <summary>
        /// Retorna a pasta do executável
        /// </summary>
        /// <returns>Retorna a pasta onde está o executável</returns>
        private static string _PastaExecutavel = string.Empty;
        public static string PastaExecutavel
        {
            get
            {
                if (string.IsNullOrEmpty(_PastaExecutavel))
                    return System.IO.Path.GetDirectoryName(Application.ExecutablePath);

                return _PastaExecutavel;
            }
            set
            {
                _PastaExecutavel = value;
            }
        }

        #region Pastas de comunicação geral do ERP com o UniNFe
        /// <summary>
        /// Pasta de comunicação geral do ERP com o UniNFe (Envio)
        /// </summary>
        public static string PastaGeral
        {
            get { return Propriedade.PastaExecutavel + "\\Geral"; }
        }

        /// <summary>
        /// Pasta de comunicação geral do ERP com o UniNFe (Temporária)
        /// </summary>
        public static string PastaGeralTemporaria
        {
            get { return Propriedade.PastaGeral + "\\Temp"; }
        }

        /// <summary>
        /// Pasta de comunicação geral do ERP com o UniNFe (Retornos)
        /// </summary>
        public static string PastaGeralRetorno
        {
            get { return Propriedade.PastaGeral + "\\Retorno"; }
        }
        #endregion

        /// <summary>
        /// Retorna o XML contendos as definicoes dos webservices
        /// </summary>
        private static string _NomeArqXMLWebService_NFe = "";
        public static string NomeArqXMLWebService_NFe
        {
            get
            {
                if (string.IsNullOrEmpty(_NomeArqXMLWebService_NFe))
                    return Propriedade.PastaExecutavel + "\\NFe\\WSDL\\Webservice.xml";

                return _NomeArqXMLWebService_NFe;
            }
            set
            {
                _NomeArqXMLWebService_NFe = value;
            }
        }
        public static string NomeArqXMLWebService_NFSe
        {
            get
            {
                return Propriedade.PastaExecutavel + "\\NFse\\WSDL\\Webservice.xml";
            }
        }

        public static TipoAplicativo TipoAplicativo { get; set; }

        public static List<Municipio> Municipios { get; set; }

        private static List<Municipio> _Estados = null;
        public static List<Municipio> Estados
        {
            get
            {
                if (_Estados == null)
                {
                    _Estados = new List<Components.Municipio>();

                    if (File.Exists(NomeArqXMLWebService_NFe))
                    {
                        XElement axml = XElement.Load(NomeArqXMLWebService_NFe);
                        var s = (from p in axml.Descendants(NFe.Components.NFeStrConstants.Estado)
                                 where (Int32)p.Attribute(NFe.Components.TpcnResources.ID.ToString()) < 900
                                 orderby p.Attribute(NFe.Components.NFeStrConstants.Nome).Value
                                 select new
                                {
                                    Nome = (string)p.Attribute(NFeStrConstants.Nome),
                                    ID = (Int32)p.Attribute(TpcnResources.ID.ToString()),
                                    UF = (string)p.Attribute(TpcnResources.UF.ToString()),
                                    SVC = (string)p.Attribute(NFeStrConstants.SVC)
                                });
                        foreach (var item in s)
                        {
                            _Estados.Add(new Municipio
                            {
                                CodigoMunicipio = item.ID,
                                Nome = item.Nome,
                                UF = item.UF,
                                svc = NFe.Components.EnumHelper.StringToEnum<TipoEmissao>(item.SVC)
                            });
                        }
                    }
                }
                return _Estados;
            }
            set { _Estados = value; }
        }

        /// <summary>
        /// Retorna a pasta onde são gravados os log´s do UniNFe
        /// </summary>
        /// <returns>Pasta de log</returns>
        public static string PastaLog
        {
            get { return PastaExecutavel + "\\log"; }
        }

        /// <summary>
        /// Retorna a pasta dos schemas para validar os XML´s
        /// </summary>
        /// <returns></returns>
        public static string PastaSchemas
        {
            get { return PastaExecutavel + "\\schemas"; }
        }

        public class ExtensaoClass
        {
            public string EnvioXML { get; set; }
            public string EnvioTXT { get; set; }
            public string RetornoXML { get; set; }
            public string RetornoTXT { get; set; }
            public string descricao { get; set; }

            public ExtensaoClass(string exml, string etxt, string rxml, string rtxt, string descr)
            {
                this.EnvioXML = exml;
                this.EnvioTXT = etxt;
                this.descricao = descr;
                this.RetornoTXT = rtxt;
                this.RetornoXML = rxml;
            }
        }

        public enum TipoEnvio
        {
            AltCon,
            ConsCertificado,
            ConsInf,
            GerarChaveNFe,
            /// <summary>
            /// NFe
            /// </summary>
            NFe,
            ConsNFeDest,
            EnvCancelamento,
            EnvCCe,
            EnvDownload,
            EnvManifestacao,
            /// <summary>
            /// CTe
            /// </summary>
            CTe,
            /// <summary>
            /// MDFe
            /// </summary>
            MDFe,
            MDFeConsNaoEncerrados,
            /// <summary>
            /// NFSe
            /// </summary>
            EnvLoteRps,
            PedCanNFSe,
            PedInuNFSe,
            PedLoteRps,
            PedNFSePDF,
            PedNFSePNG,
            PedNFSeXML,
            PedSitLoteRps,
            PedSitNFSe,
            PedSitNFSeRps,
            PedURLNFSe,
            PedURLNFSeSerie,
            /// <summary>
            /// Extensões em comum entre NFe, CTe e MDF-e
            /// </summary>
            ConsCad,
            EnvDFe,
            EnvDanfeReport,
            EnvImpressaoDanfe,
            EnvLot,
            EnvWSExiste,
            LMC,
            MontarLote,
            PedEve,
            PedInu,
            PedRec,
            PedSit,
            PedSta,
            ///
            /// Exporadicos
            /// 
            cce_XML,
            cancel_XML,
            sair_XML,
            pedLayouts,
            pedUpdatewsdl,
            pedRestart
        }
        private static Dictionary<TipoEnvio, ExtensaoClass> ListaExtensoes = new Dictionary<TipoEnvio, ExtensaoClass>();
        public static ExtensaoClass Extensao(TipoEnvio value)
        {
            if (ListaExtensoes.Count == 0)
            {
                #region Extensões gerais
                ListaExtensoes.Add(TipoEnvio.AltCon, new ExtensaoClass(
                    "-alt-con.xml", "-alt-con.txt", 
                    "-ret-alt-con.xml", "-ret-alt-con.txt", 
                    "Alteração de configuração de empresas"));
                
                ListaExtensoes.Add(TipoEnvio.ConsCertificado, new ExtensaoClass(
                    "-cons-certificado.xml", null, 
                    "uninfe-ret-cons-certificado.xml", null, 
                    "Consulta aos certificados instalados"));
                
                ListaExtensoes.Add(TipoEnvio.ConsInf, new ExtensaoClass(
                    "-cons-inf.xml", "-cons-inf.txt", 
                    "-ret-cons-inf.xml", "-ret-cons-inf.txt", 
                    "Consulta as configurações do UniNFe"));
                #endregion

                #region Extensões da NFe
                ListaExtensoes.Add(TipoEnvio.ConsNFeDest, new ExtensaoClass(
                    "-cons-nfe-dest.xml",
                    "-cons-nfe-dest.txt",
                    "-ret-cons-nfe-dest.xml",
                    "",
                    "Consulta de notas destinadas"));

                ListaExtensoes.Add(TipoEnvio.EnvCancelamento, new ExtensaoClass(
                    "-env-canc.xml", "-env-canc.txt", 
                    "-ret-env-canc.xml", "", 
                    "Pedido de cancelamento de NFe/NFCe, use a extensão -ped-eve.xml ou -ped-eve.txt"));
                
                ListaExtensoes.Add(TipoEnvio.EnvCCe, new ExtensaoClass(
                    "-env-cce.xml","-env-cce.txt",
                    "-ret-env-cce.xml", "", 
                    "Carta de correção, use a extensao -ped-eve.xml ou -ped-eve.txt"));
                
                ListaExtensoes.Add(TipoEnvio.EnvManifestacao, new ExtensaoClass(
                    "-env-manif.xml", "-env-manif.txt",
                    "-ret-env-manif.xml", "",
                    "Pedido de manifestação, use a extensao -ped-eve.xml ou -ped-eve.txt"));

                ListaExtensoes.Add(TipoEnvio.EnvDownload, new ExtensaoClass(
                    "-nfe-down.xml", "-nfe-down.txt",
                    "-ret-nfe-down.xml", "",
                    "Pedido de download de NFe"));

                ListaExtensoes.Add(TipoEnvio.GerarChaveNFe, new ExtensaoClass(
                    "-gerar-chave.xml", "-gerar-chave.txt",
                    "-ret-gerar-chave.xml", "-ret-gerar-chave.txt",
                    "Pedido de geração da chave de acesso da NFe/NFCe/MDFe/CTe"
                ));
                
                ListaExtensoes.Add(TipoEnvio.NFe, new ExtensaoClass(
                    "-nfe.xml", "-nfe.txt",
                    "-nfe-ret.xml", "",
                    "Pedido de envio de NFe/NFCe"
                ));

                #endregion

                #region Extensões do CTe
                ListaExtensoes.Add(TipoEnvio.CTe, new ExtensaoClass(
                    "-cte.xml", "",
                    "", "",
                    "Pedido de emissão de CTe"));
                #endregion

                #region Extensões do MDFe
                ListaExtensoes.Add(TipoEnvio.MDFe, new ExtensaoClass(
                    "-mdfe.xml", "",
                    "", "",
                    "Pedido de emissão do MDFe"));

                ListaExtensoes.Add(TipoEnvio.MDFeConsNaoEncerrados, new ExtensaoClass(
                    "-ped-cons-mdfe-naoenc.xml", "",
                    "-ret-cons-mdfe-naoenc.xml", "",
                    "Consulta de MDFe não encerrados"));
                #endregion

                #region Extensoes da NFSe
                ListaExtensoes.Add(TipoEnvio.EnvLoteRps, new ExtensaoClass(
                    "-env-loterps.xml", "",
                    "-ret-loterps.xml", "",
                    "Envio de lote/rps (NFSe)"));

                ListaExtensoes.Add(TipoEnvio.PedCanNFSe, new ExtensaoClass(
                    "-ped-cannfse.xml", "",
                    "-cannfse.xml", "",
                    "Pedido de cancelamento (NFSe)"));

                ListaExtensoes.Add(TipoEnvio.PedLoteRps, new ExtensaoClass(
                    "-ped-loterps.xml", "",
                    "-loterps.xml", "",
                    "Envio de consulta de lote/rps (NFSe)" ));

                ListaExtensoes.Add(TipoEnvio.PedSitLoteRps, new ExtensaoClass(
                    "-ped-sitloterps.xml", "",
                    "-sitloterps.xml", "",
                    "Pedido de situação de lote/rps (NFSe)"));

                ListaExtensoes.Add(TipoEnvio.PedSitNFSeRps, new ExtensaoClass(
                    "-ped-sitnfserps.xml", "",
                    "-sitnfserps.xml", "",
                    "Pedido de situação do rps (NFSe)"));

                ListaExtensoes.Add(TipoEnvio.PedSitNFSe, new ExtensaoClass(
                    "-ped-sitnfse.xml", "",
                    "-sitnfse.xml", "",
                    "Pedido de situação da nota (NFSe)"));

                ListaExtensoes.Add(TipoEnvio.PedURLNFSe, new ExtensaoClass(
                    "-ped-urlnfse.xml", "",
                    "-urlnfse.xml", "",
                    "XML de Consulta da URL de Visualização da NFSe"));

                ListaExtensoes.Add(TipoEnvio.PedURLNFSeSerie, new ExtensaoClass(
                    "-ped-urlnfseserie.xml", "",
                    "-urlnfseserie.xml", "",
                    "XML de Consulta da URL de Visualização da NFSe-Serie"));

                ListaExtensoes.Add(TipoEnvio.PedNFSePDF, new ExtensaoClass(
                    "-ped-nfsepdf.xml", "",
                    "-nfsepdf.xml", "",
                    "Pedido do link da NFSe"));

                ListaExtensoes.Add(TipoEnvio.PedNFSeXML, new ExtensaoClass(
                    "-ped-nfsexml.xml", "",
                    "-nfsexml.xml", "",
                    "Pedido do XML da NFSe"
                    ));

                ListaExtensoes.Add(TipoEnvio.PedNFSePNG, new ExtensaoClass(
                    "-ped-nfsepng.xml", "",
                    "-nfsepng.xml", "",
                    "Pedido do link da NFSe"));

                ListaExtensoes.Add(TipoEnvio.PedInuNFSe, new ExtensaoClass(
                    "-ped-inunfse.xml", "",
                    "-inunfse.xml", "",
                    "Inutilização de NFSe"));
                #endregion

                #region Extensões em comum entre NFe, CTe e MDF-e
                ListaExtensoes.Add(TipoEnvio.ConsCad, new ExtensaoClass(
                    "-cons-cad.xml",
                    "-cons-cad.txt",
                    "-ret-cons-cad.xml",
                    "",
                    "Consulta ao cadastro de contribuinte"));

                ListaExtensoes.Add(TipoEnvio.EnvLot, new ExtensaoClass(
                    "-env-lot.xml", "",
                    "", "",
                    "Pedido de envio de lote de NFe/NFCe/MDFe/CTe"));

                ListaExtensoes.Add(TipoEnvio.LMC, new ExtensaoClass(
                    "-lmc.xml", "",
                    "-ret-lmc.xml", "",
                    "Movimentação de Combustíveis (LMC)"));

                ListaExtensoes.Add(TipoEnvio.MontarLote, new ExtensaoClass(
                    "-montar-lote.xml", "-montar-lote.txt",
                    "", "",
                    "Criação de XML contendo várias NFe/NFCe/MDFe/CTe"));

                ListaExtensoes.Add(TipoEnvio.PedEve, new ExtensaoClass(
                    "-ped-eve.xml", "-ped-eve.txt",
                    "-eve.xml", "",
                    "Pedido de eventos associados a NFe/NFCe/MDFe/CTe"));

                ListaExtensoes.Add(TipoEnvio.PedInu, new ExtensaoClass(
                    "-ped-inu.xml", "-ped-inu.txt",
                    "-inu.xml", "",
                    "Pedido de inutilização de NFe/NFCe/CTe"));

                ListaExtensoes.Add(TipoEnvio.PedRec, new ExtensaoClass(
                    "-ped-rec.xml", "",
                    "-pro-rec.xml", "",
                    "Pedido do recibo da NFe/NFCe"));

                ListaExtensoes.Add(TipoEnvio.PedSit, new ExtensaoClass(
                    "-ped-sit.xml",
                    "-ped-sit.txt",
                    "-sit.xml",
                    "",
                    "Pedido de situação da NFe/NFCe/MDFe/CTe"));

                ListaExtensoes.Add(TipoEnvio.PedSta, new ExtensaoClass(
                    "-ped-sta.xml",
                    "-ped-sta.txt",
                    "-sta.xml",
                    "",
                    "Pedido de situação de serviços de NFe/NFCe/MDFe/CTe junto a Sefaz"));

                ListaExtensoes.Add(TipoEnvio.EnvWSExiste, new ExtensaoClass(
                    "-env-ws.xml",
                    "-env-ws.txt",
                    "-ret-env-ws.xml",
                    "-ret-env-ws.txt",
                    "Pesquisa de se um serviço existe para um determinado estado (Producao/Homologacao)"));

                ListaExtensoes.Add(TipoEnvio.EnvDanfeReport, new ExtensaoClass(
                    "-env-danfe-report.xml",
                    "-env-danfe-report.txt",
                    "-ret-env-danfe-report.xml",
                    "-ret-env-danfe-report.txt",
                    "Pedido de solicitação do relatório de e-mail enviado pelo DAMFe/DACTe/DAMDFe/CCe"));

                ListaExtensoes.Add(TipoEnvio.EnvImpressaoDanfe, new ExtensaoClass(
                    "-env-danfe.xml",
                    "-env-danfe.txt",
                    "-ret-danfe.xml",
                    "-ret-danfe.txt",
                    "Pedido de impressão do DAMFe/DACTe/DAMDFe/CCe"));

                ListaExtensoes.Add(TipoEnvio.EnvDFe, new ExtensaoClass(
                    "-con-dist-dfe.xml",
                    "-con-dist-dfe.txt",
                    "-dist-dfe.xml",
                    "-dist-dfe.txt",
                    "Consulta de DFe"));
                #endregion

                #region Exporadicos

                ListaExtensoes.Add(TipoEnvio.pedUpdatewsdl, new ExtensaoClass("-updatewsdl.xml", "-updatewsdl.txt", "-ret-updatewsdl.xml", "-ret-updatewsdl.txt", "Executar a atualização dos WSDL's e Schemas"));
                ListaExtensoes.Add(TipoEnvio.pedRestart, new ExtensaoClass("-restart.xml", "-restart.txt", "", "", "Reiniciar o UniNFe."));
                ListaExtensoes.Add(TipoEnvio.sair_XML, new ExtensaoClass("-sair.xml", "-sair.txt", "", "", "Fechar o UniNFe"));
                ListaExtensoes.Add(TipoEnvio.cce_XML, new ExtensaoClass("-cce.xml", "", "", "", "Uso específico, não usar"));
                ListaExtensoes.Add(TipoEnvio.cancel_XML, new ExtensaoClass("-cancel.xml", "", "", "", "Uso específico, não usar"));
                ListaExtensoes.Add(TipoEnvio.pedLayouts, new ExtensaoClass("-layouts.xml", "-layouts.txt", "-ret-layouts.pdf", "", "Gerar um PDF com o layout da NFe em TXT e extensões usadas no UniNFe."));
                #endregion
            }
            return ListaExtensoes[value];
        }

        #region Propriedades com as extensões dos XML ou TXT de envio
        /// <summary>
        /// Classe com as propriedades com as extensões dos XML ou TXT de envio
        /// </summary>
        public class ExtEnvio
        {
            #region Extensoes que so estao aqui para quem utiliza o codigo em seus projetos

            #region Extensões gerais
            /// <summary>
            /// -cons-certificado.xml
            /// </summary>
            public static string ConsCertificado = Extensao(TipoEnvio.ConsCertificado).EnvioXML;
            ///// <summary>
            ///// -alt-con.xml | -alt-con.txt
            ///// </summary>
            public static string AltCon_XML = Extensao(TipoEnvio.AltCon).EnvioXML;
            public static string AltCon_TXT = Extensao(TipoEnvio.AltCon).EnvioTXT;
            /// <summary>
            /// -cons-inf.xml | -cons-nf.txt
            /// </summary>
            public static string ConsInf_XML = Extensao(TipoEnvio.ConsInf).EnvioXML;
            public static string ConsInf_TXT = Extensao(TipoEnvio.ConsInf).EnvioTXT;
            #endregion

            #region Extensões da NFe
            /// <summary>
            /// -nfe.xml
            /// </summary>
            public static string NFe = Extensao(TipoEnvio.NFe).EnvioXML;
            
            /// <summary>
            /// -nfe.txt
            /// </summary>
            public static string NFe_TXT = Extensao(TipoEnvio.NFe).EnvioTXT;
            
            /// <summary>
            /// -env-cce.xml
            /// </summary>
            public static string EnvCCe = Extensao(TipoEnvio.EnvCCe).EnvioXML;

            /// <summary>
            /// -env-cce.txt
            /// </summary>
            public static string EnvCCe_TXT = Extensao(TipoEnvio.EnvCCe).EnvioTXT;
            
            /// <summary>
            /// -env-manif.xml
            /// </summary>
            public static string EnvManifestacao = Extensao(TipoEnvio.EnvManifestacao).EnvioTXT;
            
            /// <summary>
            /// -env-manif.txt
            /// </summary>
            public static string EnvManifestacao_TXT = Extensao(TipoEnvio.EnvManifestacao).EnvioTXT;
            
            /// <summary>
            /// -env-canc.xml
            /// </summary>
            public static string EnvCancelamento = Extensao(TipoEnvio.EnvCancelamento).EnvioXML;
            
                /// <summary>
            /// -env-canc.txt
            /// </summary>
            public static string EnvCancelamento_TXT = Extensao(TipoEnvio.EnvCancelamento).EnvioTXT;
            
            /// <summary>
            /// -gerar-chave.xml
            /// </summary>
            public static string GerarChaveNFe = Extensao(TipoEnvio.GerarChaveNFe).EnvioXML;
            
            /// <summary>
            /// -gerar-chave.txt
            /// </summary>
            public static string GerarChaveNFe_TXT = Extensao(TipoEnvio.GerarChaveNFe).EnvioTXT;
            
            /// <summary>
            /// -down-nfe.xml
            /// </summary>
            public static string EnvDownload_XML = Extensao(TipoEnvio.EnvDownload).EnvioXML;
            
            /// <summary>
            /// -down-nfe.txt
            /// </summary>
            public static string EnvDownload_TXT = Extensao(TipoEnvio.EnvDownload).EnvioTXT;
            
            /// <summary>
            /// -cons-nfe-dest.xml
            /// </summary>
            public static string ConsNFeDest_XML = Extensao(TipoEnvio.ConsNFeDest).EnvioXML;
            
            /// <summary>
            /// -cons-nfe-dest.txt
            /// </summary>
            public static string ConsNFeDest_TXT = Extensao(TipoEnvio.ConsNFeDest).EnvioTXT;
            #endregion

            #region Extensões do CTe
            /// <summary>
            /// -cte.xml
            /// </summary>
            public static string CTe = Extensao(TipoEnvio.CTe).EnvioXML;
            #endregion

            #region Extensões da NFS-e
            /// <summary>
            /// -env-loterps.xml X -ret-loterps.xml
            /// </summary>
            public static string EnvLoteRps = Extensao(TipoEnvio.EnvLoteRps).EnvioXML;

            /// <summary>
            /// -ped-cannfse.xml
            /// </summary>
            public static string PedCanNfse = Extensao(TipoEnvio.PedCanNFSe).EnvioXML;

            /// <summary>
            /// -ped-loterps.xml
            /// </summary>
            public static string PedLoteRps = Extensao(TipoEnvio.PedLoteRps).EnvioXML;

            /// <summary>
            /// -ped-sitloterps.xml
            /// </summary>
            public static string PedSitLoteRps = Extensao(TipoEnvio.PedSitLoteRps).EnvioXML;

            /// <summary>
            /// -ped-sitnfserps.xml
            /// </summary>
            public static string PedSitNfseRps = Extensao(TipoEnvio.PedSitNFSeRps).EnvioXML;

            /// <summary>
            /// -ped-sitnfse.xml
            /// </summary>
            public static string PedSitNfse = Extensao(TipoEnvio.PedSitNFSe).EnvioXML;

            /// <summary>
            /// -ped-urlnfse.xml x -urlnfse.xml
            /// </summary>
            public static string PedURLNfse = Extensao(TipoEnvio.PedURLNFSe).EnvioXML;

            /// <summary>
            /// -ped-urlnfseserie.xml x -urlnfseserie.xml
            /// </summary>
            public static string PedURLNfseSerie = Extensao(TipoEnvio.PedURLNFSeSerie).EnvioXML;

            /// <summary>
            /// -ped-nfsepng.xml x -nfsepng.xml
            /// </summary>
            public static string PedNfsePNG = Extensao(TipoEnvio.PedNFSePNG).EnvioXML;
            
            /// <summary>
            /// -ped-inunfse.xml x -inunfse.xml
            /// </summary>
            public static string PedInuNfse = Extensao(TipoEnvio.PedInuNFSe).EnvioXML;

            /// <summary>
            /// -ped-nfsepdf.xml x -nfsepdf.xml
            /// </summary>
            public static string PedNfsePDF = Extensao(TipoEnvio.PedNFSePDF).EnvioXML;
            #endregion

            #region Extensões MDF-e
            /// <summary>
            /// -mdfe.xml
            /// </summary>
            public static string MDFe = Extensao(TipoEnvio.MDFe).EnvioXML;

            /// <summary>
            /// -ped-cons-mdfe-naoenc.xml
            /// </summary>
            public static string MDFeConsNaoEncerrados = Extensao(TipoEnvio.MDFeConsNaoEncerrados).EnvioXML;
            #endregion

            #region Extensões em comum entre NFe, CTe e MDF-e

            /// <summary>
            /// -ped-eve.xml
            /// </summary>
            public static string PedEve = Extensao(TipoEnvio.PedEve).EnvioXML;

            /// <summary>
            /// -ped-eve.txt
            /// </summary>
            public static string PedEve_TXT = Extensao(TipoEnvio.PedEve).EnvioTXT;
            
            /// <summary>
            /// -montar-lote.xml
            /// </summary>
            public static string MontarLote = Extensao(TipoEnvio.MontarLote).EnvioXML;

            /// <summary>
            /// -montar-lote.txt
            /// </summary>
            public static string MontarLote_TXT = Extensao(TipoEnvio.MontarLote).EnvioTXT;
            
            /// <summary>
            /// -cons-cad.xml
            /// </summary>
            public static string ConsCad = Extensao(TipoEnvio.ConsCad).EnvioXML;
            
            /// <summary>
            /// -cons-cad.txt
            /// </summary>
            public static string ConsCad_TXT = Extensao(TipoEnvio.ConsCad).EnvioTXT;
            
            /// <summary>
            /// -env-lot.xml
            /// </summary>
            public static string EnvLot = Extensao(TipoEnvio.EnvLot).EnvioXML;

            /// <summary>
            /// -ped-inu.xml
            /// </summary>
            public static string PedInu = Extensao(TipoEnvio.PedInu).EnvioXML;
            
            /// <summary>
            /// -ped-inu.txt
            /// </summary>
            public static string PedInu_TXT = Extensao(TipoEnvio.PedInu).EnvioTXT;
            
            /// <summary>
            /// -ped-rec.xml
            /// </summary>
            public static string PedRec = Extensao(TipoEnvio.PedRec).EnvioXML;
            
            /// <summary>
            /// -ped-sit.xml
            /// </summary>
            public static string PedSit = Extensao(TipoEnvio.PedSit).EnvioXML;

            /// <summary>
            /// -ped-sit.txt
            /// </summary>
            public static string PedSit_TXT = Extensao(TipoEnvio.PedSit).EnvioTXT;
            
            /// <summary>
            /// -ped-sta.xml
            /// </summary>
            public static string PedSta = Extensao(TipoEnvio.PedSta).EnvioXML;
            
            /// <summary>
            /// -ped-sta.txt
            /// </summary>
            public static string PedSta_TXT = Extensao(TipoEnvio.PedSta).EnvioTXT;
            #endregion

            #region Extensões usadas na pesquisa de se um serviço existe para um determinado estado (Producao/Homologacao)
            public static string EnvWSExiste = Extensao(TipoEnvio.EnvWSExiste).EnvioXML;
            public static string EnvWSExiste_TXT = Extensao(TipoEnvio.EnvWSExiste).EnvioTXT;
            #endregion

            #region DANFE
            public static string EnvImpressaoDanfe = Extensao(TipoEnvio.EnvImpressaoDanfe).EnvioXML;
            public static string EnvImpressaoDanfe_TXT = Extensao(TipoEnvio.EnvImpressaoDanfe).EnvioTXT;

            public static string EnvDanfeReport = Extensao(TipoEnvio.EnvDanfeReport).EnvioXML;
            public static string EnvDanfeReport_TXT = Extensao(TipoEnvio.EnvDanfeReport).EnvioTXT;
            #endregion

            #region Extensoes de DFe
            public static string EnvDFe = Extensao(TipoEnvio.EnvDFe).EnvioXML;
            public static string EnvDFe_TXT = Extensao(TipoEnvio.EnvDFe).EnvioTXT;
            #endregion

            #region Extensões LMC
            public static string LMC = Extensao(TipoEnvio.LMC).EnvioXML;
            #endregion

            #region Extensões só para resolver um problema de compatibilidade de um usuário, com o tempo poderemos excluir. Wandrey 10/12/2014
            public static string cce_XML = Extensao(TipoEnvio.cce_XML).EnvioXML;
            public static string cancel_XML = Extensao(TipoEnvio.cancel_XML).EnvioXML;
            #endregion

            #endregion
        }
        #endregion

        #region Propriedades com as extensões dos XML ou TXT de retorno
        /// <summary>
        /// Classe com as propriedades com as extensões dos XML ou TXT de retorno
        /// </summary>
        public class ExtRetorno
        {
            #region Extensoes que so estao aqui para quem utiliza o codigo em seus projetos
            public static string RetAltCon_XML = Extensao(TipoEnvio.AltCon).RetornoXML;
            public static string RetAltCon_TXT = Extensao(TipoEnvio.AltCon).RetornoTXT;
            public static string RetGerarChaveNFe_XML = Extensao(TipoEnvio.GerarChaveNFe).RetornoXML;
            public static string RetGerarChaveNFe_TXT = Extensao(TipoEnvio.GerarChaveNFe).RetornoTXT;
            public static string RetConsInf_XML = Extensao(TipoEnvio.ConsInf).RetornoXML;
            public static string RetConsInf_TXT = Extensao(TipoEnvio.ConsInf).RetornoTXT;
            public static string retEnvCCe_XML = Extensao(TipoEnvio.EnvCCe).RetornoXML;
            public static string retCancelamento_XML = Extensao(TipoEnvio.EnvCancelamento).RetornoXML;
            public static string retDownload_XML = Extensao(TipoEnvio.EnvDownload).RetornoXML;
            public static string retConsNFeDest_XML = Extensao(TipoEnvio.ConsNFeDest).RetornoXML;
            public static string retManifestacao_XML = Extensao(TipoEnvio.EnvManifestacao).RetornoXML;
            public static string MDFeConsNaoEnc = Extensao(TipoEnvio.MDFeConsNaoEncerrados).RetornoXML;
            public static string Eve = Extensao(TipoEnvio.PedEve).RetornoXML;
            public static string Inu_XML = Extensao(TipoEnvio.PedInu).RetornoXML;
            public static string ConsCad_XML = Extensao(TipoEnvio.ConsCad).RetornoXML;
            public static string RetEnvLoteRps = Extensao(TipoEnvio.EnvLoteRps).RetornoXML;
            public static string CanNfse = Extensao(TipoEnvio.PedCanNFSe).RetornoXML;
            public static string Sit_XML = Extensao(TipoEnvio.PedSit).RetornoXML;
            public static string ProRec_XML = Extensao(TipoEnvio.PedRec).RetornoXML;
            public static string Sta_XML = Extensao(TipoEnvio.PedSta).RetornoXML;
            public static string LoteRps = Extensao(TipoEnvio.PedLoteRps).RetornoXML;
            public static string SitLoteRps = Extensao(TipoEnvio.PedSitLoteRps).RetornoXML;
            public static string SitNfse = Extensao(TipoEnvio.PedSitNFSe).RetornoXML;
            public static string SitNfseRps = Extensao(TipoEnvio.PedSitNFSeRps).RetornoXML;
            public static string Urlnfse = Extensao(TipoEnvio.PedURLNFSe).RetornoXML;
            public static string NFSePNG = Extensao(TipoEnvio.PedNFSePNG).RetornoXML;
            public static string InuNfse = Extensao(TipoEnvio.PedInuNFSe).RetornoXML;
            public static string NFSePDF = Extensao(TipoEnvio.PedNFSePDF).RetornoXML;
            public static string retWSExiste_XML = Extensao(TipoEnvio.EnvWSExiste).RetornoXML;
            public static string retWSExiste_TXT = Extensao(TipoEnvio.EnvWSExiste).RetornoTXT;
            public static string RetImpressaoDanfe_XML = Extensao(TipoEnvio.EnvImpressaoDanfe).RetornoXML;
            public static string RetImpressaoDanfe_TXT = Extensao(TipoEnvio.EnvImpressaoDanfe).RetornoTXT;
            public static string RetDanfeReport_XML = Extensao(TipoEnvio.EnvDanfeReport).RetornoXML;
            public static string RetDanfeReport_TXT = Extensao(TipoEnvio.EnvDanfeReport).RetornoTXT;
            public static string retEnvDFe_XML = Extensao(TipoEnvio.EnvDFe).RetornoXML;
            public static string retEnvDFe_TXT = Extensao(TipoEnvio.EnvDFe).RetornoTXT;
            public static string LMCRet = Extensao(TipoEnvio.LMC).RetornoXML;
            #endregion

            #region Extensões gerais

            #endregion

            #region Extensões NFe
            /// <summary>
            /// -procnfe.xml
            /// </summary>
            public const string ProcNFe = "-procNFe.xml"; //Não deixar tudo minusculo para evitar problemas com Linux configurado para Case Sensitive. Wandrey 23/06/2011
            
            /// <summary>
            /// -den.xml
            /// </summary>
            public const string Den = "-den.xml";
            
            /// <summary>
            /// -ret-env-cce.err
            /// </summary>
            public const string retEnvCCe_ERR = "-ret-env-cce.err";
            
            /// <summary>
            /// -ret-canc.err
            /// </summary>
            public const string retCancelamento_ERR = "-ret-env-canc.err";
            
            /// <summary>
            /// -procEventoNFe.xml
            /// </summary>
            public const string ProcEventoNFe = "-procEventoNFe.xml";
            
            /// <summary>
            /// -procinutnfe.xml
            /// </summary>
            public const string ProcInutNFe = "-procInutNFe.xml"; //Não deixar tudo minusculo para evitar problemas com Linux configurado para Case Sensitive. Wandrey 23/06/2011
            
            /// <summary>
            /// -nfe.err
            /// </summary>
            public const string Nfe_ERR = "-nfe.err";
            
            /// <summary>
            /// -ret-down-nfe.err
            /// </summary>
            public const string retDownload_ERR = "-ret-nfe-down.err";
            
            /// <summary>
            /// -ret-cons-nfe-dest.err
            /// </summary>
            public const string retConsNFeDest_ERR = "-ret-cons-nfe-dest.err";
            
            /// <summary>
            /// -ret-manif.err
            /// </summary>
            public const string retManifestacao_ERR = "-ret-env-manif.err";
            #endregion

            #region Extensões CTe
            
            /// <summary>
            /// -procCTe.xml
            /// </summary>
            public const string ProcCTe = "-procCTe.xml"; //Não deixar tudo minusculo para evitar problemas com Linux configurado para Case Sensitive. Wandrey 23/06/2011
            
            /// <summary>
            /// -procEventoCTe.xml
            /// </summary>
            public const string ProcEventoCTe = "-procEventoCTe.xml";
            
            /// <summary>
            /// -procInutCTe.xml
            /// </summary>
            public static string ProcInutCTe = "-procInutCTe.xml"; //Não deixar tudo minusculo para evitar problemas com Linux configurado para Case Sensitive. Wandrey 23/06/2011
            
            /// <summary>
            /// -cte.err
            /// </summary>
            public static string Cte_ERR = "-cte.err";
            #endregion

            #region Extensões MDFe
            /// <summary>
            /// -procMDFe.xml
            /// </summary>
            public const string ProcMDFe = "-procMDFe.xml"; //Não deixar tudo minusculo para evitar problemas com Linux configurado para Case Sensitive. Wandrey 23/06/2011
            
            /// <summary>
            /// -procEventoMDFe.xml
            /// </summary>
            public const string ProcEventoMDFe = "-procEventoMDFe.xml";
            
            /// <summary>
            /// -mdfe.err
            /// </summary>
            public const string MDFe_ERR = "-mdfe.err";
            
            /// <summary>
            /// -ret-consmdfenaoenc.err
            /// </summary>
            public static string MDFeConsNaoEnc_ERR = "-ret-cons-mdfe-naoenc.err";
            #endregion

            #region Extensões em comum entre NFe, CTe e MDF-e
            /// <summary>
            /// -eve.err
            /// </summary>
            public const string Eve_ERR = "-eve.err";
            
            /// <summary>
            /// -montar-lote.err
            /// </summary>
            public const string MontarLote_ERR = "-montar-lote.err";
            
            /// <summary>
            /// -ret-cons-cad.err
            /// </summary>
            public const string ConsCad_ERR = "-ret-cons-cad.err";
            
            /// <summary>
            /// -sit.err
            /// </summary>
            public const string Sit_ERR = "-sit.err";
            
            /// <summary>
            /// -pro-rec.err
            /// </summary>
            public const string ProRec_ERR = "-pro-rec.err";
            
            /// <summary>
            /// -sta.err
            /// </summary>
            public const string Sta_ERR = "-sta.err";
            
            /// <summary>
            /// -inu.err
            /// </summary>
            public const string Inu_ERR = "-inu.err";
            
            /// <summary>
            /// -rec.xml
            /// </summary>
            public const string Rec = "-rec.xml";
            
            /// <summary>
            /// -rec.err
            /// </summary>
            public const string Rec_ERR = "-rec.err";
            #endregion

            #region Extensões NFSe
            
            /// <summary>
            /// -ret-loterps.err
            /// </summary>
            public const string RetEnvLoteRps_ERR = "-ret-loterps.err";
            
            /// <summary>
            /// -cannfse.err
            /// </summary>
            public const string CanNfse_ERR = "-cannfse.err";
            
            /// <summary>
            /// -lotrps.err
            /// </summary>
            public const string LoteRps_ERR = "-loterps.err";
            
            /// <summary>
            /// -sitloterps.err
            /// </summary>
            public const string SitLoteRps_ERR = "-sitloterps.err";

            /// <summary>
            /// -sitnfse.err
            /// </summary>
            public const string SitNfse_ERR = "-sitnfse.err";

            /// <summary>
            /// -sitnfserps.err
            /// </summary>
            public const string SitNfseRps_ERR = "-sitnfserps.err";
            
            /// <summary>
            /// -urlnfse.xml x -ped-urlnfse.xml x -urlnfse.err
            /// </summary>
            public const string Urlnfse_ERR = "-urlnfse.err";
            public const string UrlnfseSerie_ERR = "-urlnfserie.err";

            /// <summary>
            /// nfsepng.err
            /// </summary>
            public const string NFSePNG_ERR = "-nfsepng.err";
            public const string NFSePDF_ERR = "-nfsepdf.err";
            public const string NFSeXML_ERR = "-nfsexml.err";

            /// <summary>
            /// -inunfse.err
            /// </summary>
            public const string InuNfse_ERR = "-inunfse.err";
            #endregion

            #region Extensoes de DFe
            public const string retEnvDFe_ERR = "-con-dist-dfe.err";
            #endregion

            #region Extensões do LMC
            /// <summary>
            /// -ret-lmc.err
            /// </summary>
            public static string LMCRet_ERR = "-ret-lmc.err";
            /// <summary>
            /// -procLMC.xml
            /// </summary>
            public static string ProcLMC = "-procLMC.xml";
            #endregion
        }
        #endregion

        #region NomeAplicacao
        /// <summary>
        /// Retorna o nome do aplicativo 
        /// </summary>
        /// <param name="oAssembly">Passar sempre: Assembly.GetExecutingAssembly() pois ele vai pegar o Assembly do EXE ou DLL de onde está sendo chamado o método</param>
        /// <returns>string contendo o nome do Aplicativo</returns>
        /// <by>Wandrey Mundin Ferreira</by>
        /// <date>31/07/2009</date>
        public static string NomeAplicacao
        {
            get
            {
                //Montar o nome da aplicação
                string Produto = string.Empty;

                foreach (Attribute attr in Attribute.GetCustomAttributes(AssemblyEXE))
                {
                    if (attr.GetType() == typeof(AssemblyProductAttribute))
                    {
                        Produto = ((AssemblyProductAttribute)attr).Product;
                        break;
                    }
                }

                return Produto;
            }
        }
        #endregion

        #region Versao
        /// <summary>
        /// Retorna a versão do aplicativo 
        /// </summary>
        public static string Versao
        {
            get
            {
                //Montar a versão do programa
                string versao;

                Assembly _assembly = AssemblyEXE;
                if (!ExecutandoPeloUniNFe)
                    //danasa 22/7/2011
                    //se o servico está sendo executado, pega a versão do 'uninfe.exe'
                    _assembly = System.Reflection.Assembly.LoadFile(Path.Combine(PastaExecutavel, "uninfe.exe"));

                foreach (Attribute attr in Attribute.GetCustomAttributes(_assembly))
                {
                    if (attr.GetType() == typeof(AssemblyVersionAttribute))
                    {
                        versao = ((AssemblyVersionAttribute)attr).Version;
                        break;
                    }
                }
                string delimStr = ",=";
                char[] delimiter = delimStr.ToCharArray();
                string[] strAssembly = _assembly.ToString().Split(delimiter);
                versao = strAssembly[2];

                return versao;
            }
        }
        #endregion

        public static bool ServicoRodando   //danasa 22/7/2011
        {
            get
            {
                try
                {
                    if (ServiceProcess.IsServiceInstalled(Propriedade.ServiceName))
                        return ServiceProcess.StatusService(Propriedade.ServiceName) == System.ServiceProcess.ServiceControllerStatus.Running;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Retorna o XML para salvar os parametros das telas
        /// </summary>
        public static string NomeArqXMLParams
        {
            get
            {
                return PastaExecutavel + "\\UniNFeParams.xml";
            }
        }

        #region Atributos
        /// <summary>
        /// Se pode encerrar a aplicação ou não
        /// </summary>
        public static bool EncerrarApp = false;
        #endregion

        #region DescricaoAplicacao
        /// <summary>
        /// Retorna a descrição da aplicação
        /// </summary>
        /// <returns>Descrição da aplicação</returns>
        public static string DescricaoAplicacao
        {
            get
            {
                //Montar o nome da aplicação
                string descricao = string.Empty;

                foreach (Attribute attr in Attribute.GetCustomAttributes(AssemblyEXE))
                {
                    if (attr.GetType() == typeof(AssemblyDescriptionAttribute))
                    {
                        descricao = ((AssemblyDescriptionAttribute)attr).Description;
                        break;
                    }
                }

                return descricao;
            }
        }
        #endregion

        public static void VerificaArquivos(out bool error, out string msg)
        {
            switch (NFe.Components.Propriedade.TipoAplicativo)
            {
                case TipoAplicativo.MDFe:
                case TipoAplicativo.Cte:
                case TipoAplicativo.NFCe:
                case TipoAplicativo.Nfe:
                    error = !System.IO.File.Exists(Propriedade.NomeArqXMLWebService_NFe);
                    msg = "Arquivo '" + Propriedade.NomeArqXMLWebService_NFe + "' não encontrado";
                    break;
                case TipoAplicativo.Nfse:
                    error = !System.IO.File.Exists(Propriedade.NomeArqXMLMunicipios) || !System.IO.File.Exists(Propriedade.NomeArqXMLWebService_NFSe);
                    msg = "Arquivos '" + Propriedade.NomeArqXMLMunicipios + "' e/ou '" + Propriedade.NomeArqXMLWebService_NFSe + "' não encontrados";
                    break;
                default:
                    error = !System.IO.File.Exists(Propriedade.NomeArqXMLMunicipios) || !System.IO.File.Exists(Propriedade.NomeArqXMLWebService_NFSe) || !System.IO.File.Exists(Propriedade.NomeArqXMLWebService_NFe);
                    msg = "Arquivos '" + Propriedade.NomeArqXMLMunicipios + "', '" + Propriedade.NomeArqXMLWebService_NFSe + "' e/ou '" + Propriedade.NomeArqXMLWebService_NFe + "' não encontrados";
                    break;
            }
        }
    }
}
