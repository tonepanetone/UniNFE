﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading;
using NFe.Exceptions;
using NFe.Settings;
using NFe.Components;
using NFe.Validate;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net;

namespace NFe.Service
{
    /// <summary>
    /// Classe para invocar os métodos e propriedades das classes dos webservices da NFE
    /// </summary>
    public class InvocarObjeto
    {
        #region Objetos
        private Auxiliar oAux = new Auxiliar();
        #endregion

        #region Métodos

        #region Invocar()
        /// <summary>
        /// Metodo responsável por invocar o serviço do WebService do SEFAZ
        /// </summary>
        /// <param name="wsProxy">Objeto da classe construida do WSDL</param>
        /// <param name="servicoWS">Objeto da classe de envio do XML</param>
        /// <param name="metodo">Método da classe de envio do XML que faz o envio</param>
        /// <param name="cabecMsg">Objeto da classe de cabecalho do serviço</param>
        /// <param name="servicoNFe">Objeto do Serviço de envio da NFE do UniNFe</param>
        /// <param name="finalArqEnvio">string do final do arquivo a ser enviado. Sem a extensão ".xml"</param>
        /// <param name="finalArqRetorno">string do final do arquivo a ser gravado com o conteúdo do retorno. Sem a extensão ".xml"</param>
        /// <param name="gravaRetorno">Grava o arquivo de retorno para o ERP na execução deste método?</param>
        public void Invocar(WebServiceProxy wsProxy,
                            object servicoWS,
                            string metodo,
                            object cabecMsg,
                            object servicoNFe,
                            string finalArqEnvio,
                            string finalArqRetorno,
                            bool gravaRetorno,
                            SecurityProtocolType securityProtocolType)
        {
            int emp = Empresas.FindEmpresaByThread();

            finalArqEnvio = Functions.ExtractExtension(finalArqEnvio);
            finalArqRetorno = Functions.ExtractExtension(finalArqRetorno);

            XmlDocument docXML = new XmlDocument();

            // Definir o tipo de serviço da NFe
            Type typeServicoNFe = servicoNFe.GetType();

            Servicos servico = (Servicos)wsProxy.GetProp(servicoNFe, NFe.Components.NFeStrConstants.Servico);

            // Resgatar o nome do arquivo XML a ser enviado para o webservice
            string XmlNfeDadosMsg = (string)(typeServicoNFe.InvokeMember("NomeArquivoXML", System.Reflection.BindingFlags.GetProperty, null, servicoNFe, null));

            // Exclui o Arquivo de Erro
            Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + Functions.ExtrairNomeArq(XmlNfeDadosMsg, finalArqEnvio + ".xml") + finalArqRetorno + ".err");

            // Validar o Arquivo XML
            ValidarXML validar = new ValidarXML(XmlNfeDadosMsg, Empresas.Configuracoes[emp].UnidadeFederativaCodigo, false);

            string cResultadoValidacao = validar.ValidarArqXML(XmlNfeDadosMsg);
            if (cResultadoValidacao != "")
            {
                throw new Exception(cResultadoValidacao);
            }

            // Montar o XML de Lote de envio de Notas fiscais
            docXML.Load(XmlNfeDadosMsg);

            // Definir Proxy
            if (ConfiguracaoApp.Proxy)
            {
                wsProxy.SetProp(servicoWS, "Proxy", Proxy.DefinirProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta, ConfiguracaoApp.DetectarConfiguracaoProxyAuto));
            }

            // Limpa a variável de retorno
            XmlNode XmlRetorno = null;
            string strRetorno = string.Empty;

            //Vou mudar o timeout para evitar que demore a resposta e o uninfe aborte antes de recebe-la. Wandrey 17/09/2009
            //Isso talvez evite de não conseguir o número do recibo se o serviço do SEFAZ estiver lento.
            wsProxy.SetProp(servicoWS, "Timeout", 60000);

            //Verificar antes se tem conexão com a internet, se não tiver já gera uma exceção no padrão já esperado pelo ERP
            if (ConfiguracaoApp.ChecarConexaoInternet)
                if (!Functions.IsConnectedToInternet())
                {
                    //Registrar o erro da validação para o sistema ERP
                    throw new ExceptionSemInternet(ErroPadrao.FalhaInternet, "\r\nArquivo: " + XmlNfeDadosMsg);
                }

            //Atribuir conteúdo para uma propriedade da classe NfeStatusServico2
            switch (servico)
            {
                case Servicos.MDFePedidoConsultaSituacao:
                case Servicos.MDFePedidoSituacaoLote:
                case Servicos.MDFeEnviarLote:
                case Servicos.MDFeConsultaStatusServico:
                case Servicos.MDFeRecepcaoEvento:
                case Servicos.MDFeConsultaNaoEncerrado:
                    wsProxy.SetProp(servicoWS, "mdfeCabecMsgValue", cabecMsg);
                    break;

                case Servicos.CTeInutilizarNumeros:
                case Servicos.CTePedidoConsultaSituacao:
                case Servicos.CTePedidoSituacaoLote:
                case Servicos.CTeEnviarLote:
                case Servicos.CTeRecepcaoEvento:
                case Servicos.CTeConsultaStatusServico:
                    if (wsProxy.GetProp(cabecMsg, NFe.Components.TpcnResources.cUF.ToString()).ToString() == "50") //Mato Grosso do Sul fugiu o padrão nacional
                    {
                        try
                        {
                            wsProxy.SetProp(servicoWS, "cteCabecMsg", cabecMsg);
                        }
                        catch //Se der erro é pq não está no ambiente normal então tem que ser o nome padrão pois Mato Grosso do Sul fugiu o padrão nacional.
                        {
                            wsProxy.SetProp(servicoWS, "cteCabecMsgValue", cabecMsg);
                        }
                    }
                    else
                    {
                        wsProxy.SetProp(servicoWS, "cteCabecMsgValue", cabecMsg);
                    }
                    break;

                case Servicos.DFeEnviar:
                    break;

                case Servicos.LMCAutorizacao:
                    break;

                default:
                    wsProxy.SetProp(servicoWS, "nfeCabecMsgValue", cabecMsg);
                    break;
            }

            //Definir novamente o protocolo de segurança, pois é uma propriedade estática e o seu valor pode ser alterado antes do envio. Wandrey 03/05/2016
            ServicePointManager.SecurityProtocol = securityProtocolType;

            // Envio da NFe Compactada - Renan
            if (servico == Servicos.NFeEnviarLoteZip2)
            {
                XmlNfeDadosMsg = XmlNfeDadosMsg + ".gz";
                FileInfo XMLNfeZip = new FileInfo(XmlNfeDadosMsg);
                string encodedData = StreamExtensions.ToBase64(XMLNfeZip);
                XmlRetorno = wsProxy.InvokeXML(servicoWS, metodo, new object[] { encodedData });
            }
            else
                XmlRetorno = wsProxy.InvokeXML(servicoWS, metodo, new object[] { docXML });

            if (XmlRetorno == null)
                throw new Exception("Erro de envio da solicitação do serviço: " + servico.ToString());

            typeServicoNFe.InvokeMember("vStrXmlRetorno", System.Reflection.BindingFlags.SetProperty, null, servicoNFe, new object[] { XmlRetorno.OuterXml });

            // Registra o retorno de acordo com o status obtido
            //if (finalArqEnvio != string.Empty && finalArqRetorno != string.Empty)
            //{
            if (gravaRetorno)
            {
                typeServicoNFe.InvokeMember("XmlRetorno", System.Reflection.BindingFlags.InvokeMethod, null, servicoNFe, new Object[] { finalArqEnvio + ".xml", finalArqRetorno + ".xml" });
            }
            //}
        }
        #endregion

        #region InvocarNFSe()
        /// <summary>
        /// Metodo responsável por invocar o serviço do WebService do SEFAZ
        /// </summary>
        /// <param name="wsProxy">Objeto da classe construida do WSDL</param>
        /// <param name="servicoWS">Objeto da classe de envio do XML</param>
        /// <param name="metodo">Método da classe de envio do XML que faz o envio</param>
        /// <param name="cabecMsg">Objeto da classe de cabecalho do serviço</param>
        /// <param name="servicoNFe">Objeto do Serviço de envio da NFE do UniNFe</param>
        /// <param name="finalArqEnvio">string do final do arquivo a ser enviado. Sem a extensão ".xml"</param>
        /// <param name="finalArqRetorno">string do final do arquivo a ser gravado com o conteúdo do retorno. Sem a extensão ".xml"</param>
        /// <remarks>
        /// Autor: Wandrey Mundin Ferreira
        /// Data: 17/03/2010
        /// </remarks>
        public void InvocarNFSe(WebServiceProxy wsProxy,
                            object servicoWS,
                            string metodo,
                            string cabecMsg,
                            object servicoNFe,
                            string finalArqEnvio,
                            string finalArqRetorno,
                            PadroesNFSe padraoNFSe,
                            Servicos servicoNFSe,
                            SecurityProtocolType securityProtocolType)
        {
            int emp = Empresas.FindEmpresaByThread();

            finalArqEnvio = Functions.ExtractExtension(finalArqEnvio);
            finalArqRetorno = Functions.ExtractExtension(finalArqRetorno);

            XmlDocument docXML = new XmlDocument();

            // Definir o tipo de serviço da NFe
            Type typeServicoNFe = servicoNFe.GetType();

            // Resgatar o nome do arquivo XML a ser enviado para o webservice
            string XmlNfeDadosMsg = (string)(typeServicoNFe.InvokeMember("NomeArquivoXML", System.Reflection.BindingFlags.GetProperty, null, servicoNFe, null));

            // Exclui o Arquivo de Erro
            Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + Functions.ExtrairNomeArq(XmlNfeDadosMsg, finalArqEnvio + ".xml") + finalArqRetorno + ".err");

            // Validar o Arquivo XML
            ValidarXML validar = new ValidarXML(XmlNfeDadosMsg, Empresas.Configuracoes[emp].UnidadeFederativaCodigo, false);
            string cResultadoValidacao = validar.ValidarArqXML(XmlNfeDadosMsg);
            if (cResultadoValidacao != "")
            {
                switch (padraoNFSe)
                {
                    case PadroesNFSe.ISSONLINE4R:
                        break;
                    case PadroesNFSe.SMARAPD:
                        break;
                    default:
                        throw new Exception(cResultadoValidacao);
                }

            }

            //Definir novamente o protocolo de segurança, pois é uma propriedade estática e o seu valor pode ser alterado antes do envio. Wandrey 03/05/2016
            ServicePointManager.SecurityProtocol = securityProtocolType;

            // Montar o XML de Lote de envio de Notas fiscais
            docXML.Load(XmlNfeDadosMsg);

            // Definir Proxy
            if (ConfiguracaoApp.Proxy && wsProxy != null)
            {
                switch (padraoNFSe)
                {
                    case PadroesNFSe.BETHA:
                        wsProxy.Betha.Proxy = Proxy.DefinirProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta, ConfiguracaoApp.DetectarConfiguracaoProxyAuto);
                        break;

                    default:
                        wsProxy.SetProp(servicoWS, "Proxy", Proxy.DefinirProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta, ConfiguracaoApp.DetectarConfiguracaoProxyAuto));
                        break;
                }
            }

            // Limpa a variável de retorno
            string strRetorno = string.Empty;

            //Vou mudar o timeout para evitar que demore a resposta e o uninfe aborte antes de recebe-la. Wandrey 17/09/2009
            //Isso talvez evite de não conseguir o número do recibo se o serviço do SEFAZ estiver lento.
            if (wsProxy != null)
            {
                switch (padraoNFSe)
                {
                    case PadroesNFSe.NOTAINTELIGENTE:
                    case PadroesNFSe.BETHA:
                        break;

                    default:
                        wsProxy.SetProp(servicoWS, "Timeout", 120000);
                        break;
                }
            }

            //Verificar antes se tem conexão com a internet, se não tiver já gera uma exceção no padrão já esperado pelo ERP
            if (ConfiguracaoApp.ChecarConexaoInternet)  //danasa: 12/2013
                if (!Functions.IsConnectedToInternet())
                {
                    //Registrar o erro da validação para o sistema ERP
                    throw new ExceptionSemInternet(ErroPadrao.FalhaInternet, "\r\nArquivo: " + XmlNfeDadosMsg);
                }

            //Invocar o membro
            switch (padraoNFSe)
            {
                #region Padrão BETHA
                case PadroesNFSe.BETHA:
                    switch (metodo)
                    {
                        case "ConsultarSituacaoLoteRps":
                            strRetorno = wsProxy.Betha.ConsultarSituacaoLoteRps(docXML, Empresas.Configuracoes[emp].AmbienteCodigo);
                            break;

                        case "ConsultarLoteRps":
                            strRetorno = wsProxy.Betha.ConsultarLoteRps(docXML, Empresas.Configuracoes[emp].AmbienteCodigo);
                            break;

                        case "CancelarNfse":
                            strRetorno = wsProxy.Betha.CancelarNfse(docXML, Empresas.Configuracoes[emp].AmbienteCodigo);
                            break;

                        case "ConsultarNfse":
                            strRetorno = wsProxy.Betha.ConsultarNfse(docXML, Empresas.Configuracoes[emp].AmbienteCodigo);
                            break;

                        case "ConsultarNfsePorRps":
                            strRetorno = wsProxy.Betha.ConsultarNfsePorRps(docXML, Empresas.Configuracoes[emp].AmbienteCodigo);
                            break;

                        case "RecepcionarLoteRps":
                            strRetorno = wsProxy.Betha.RecepcionarLoteRps(docXML, Empresas.Configuracoes[emp].AmbienteCodigo);
                            break;
                    }
                    break;
                #endregion

                #region NOTAINTELIGENTE
                case PadroesNFSe.NOTAINTELIGENTE:
                    //NFe.Components.PClaudioMG.api_portClient wsClaudio = (NFe.Components.PClaudioMG.api_portClient)servicoWS;

                    switch (servicoNFSe)
                    {
                        case Servicos.NFSeRecepcionarLoteRps:
                            //strRetorno = wsClaudio.RecepcionarLoteRps(docXML.OuterXml.ToString());
                            break;
                    }
                    //strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { docXML.OuterXml.ToString() });
                    break;
                #endregion

                #region Padrão ISSONLINE
                case PadroesNFSe.ISSONLINE:
                    int operacao;
                    string senhaWs = Functions.GetMD5Hash(Empresas.Configuracoes[emp].SenhaWS);

                    switch (servicoNFSe)
                    {
                        case Servicos.NFSeRecepcionarLoteRps:
                            operacao = 1;
                            break;
                        case Servicos.NFSeCancelar:
                            operacao = 2;
                            break;
                        default:
                            operacao = 3;
                            break;
                    }

                    strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { Convert.ToSByte(operacao), Empresas.Configuracoes[emp].UsuarioWS, senhaWs, docXML.OuterXml });
                    break;
                #endregion

                #region Padrão Blumenau-SC
                case PadroesNFSe.BLUMENAU_SC:
                    strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { 1, docXML.OuterXml });
                    break;
                #endregion

                #region Padrão Paulistana
                case PadroesNFSe.PAULISTANA:
                    strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { 1, docXML.OuterXml });
                    break;
                #endregion

                #region TECNOSISTEMAS
                case PadroesNFSe.TECNOSISTEMAS:
                    strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { docXML.OuterXml, cabecMsg.ToString() });
                    break;
                #endregion

                #region SMARAPD
                case PadroesNFSe.SMARAPD:
                    if (metodo == "nfdEntradaCancelar")
                        strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { Empresas.Configuracoes[emp].UsuarioWS,
                        TFunctions.EncryptSHA1(Empresas.Configuracoes[emp].SenhaWS),
                        docXML.OuterXml });
                    else
                        strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { Empresas.Configuracoes[emp].UsuarioWS,
                        TFunctions.EncryptSHA1(Empresas.Configuracoes[emp].SenhaWS),
                        Empresas.Configuracoes[emp].UnidadeFederativaCodigo,
                        docXML.OuterXml });
                    break;
                #endregion

                #region ISSWEB
                case PadroesNFSe.ISSWEB:
                    string versao = docXML.DocumentElement.GetElementsByTagName("Versao")[0].InnerText;
                    string cnpj = docXML.DocumentElement.GetElementsByTagName("CNPJCPFPrestador")[0].InnerText;
                    strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { cnpj, docXML.OuterXml, versao });
                    break;
                #endregion

                #region NA_INFORMATICA
                case PadroesNFSe.NA_INFORMATICA:
                    switch (servicoNFSe)
                    {
                        #region Recepcionar Lote RPS - Assíncrono
                        case Servicos.NFSeRecepcionarLoteRps:
                            if (Empresas.Configuracoes[emp].AmbienteCodigo == (int)TipoAmbiente.taProducao)
                            {
                                ((Components.PCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.PCorumbaMS.RecepcionarLoteRps dadosEnvio = new Components.PCorumbaMS.RecepcionarLoteRps();
                                Components.PCorumbaMS.RecepcionarLoteRpsResponse dadosRetorno = new Components.PCorumbaMS.RecepcionarLoteRpsResponse();
                                dadosEnvio.EnviarLoteRpsEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.PCorumbaMS.NfseWSService)servicoWS).RecepcionarLoteRps(dadosEnvio);
                                strRetorno = dadosRetorno.EnviarLoteRpsResposta;
                            }
                            else
                            {
                                ((Components.HCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.HCorumbaMS.RecepcionarLoteRps dadosEnvio = new Components.HCorumbaMS.RecepcionarLoteRps();
                                Components.HCorumbaMS.RecepcionarLoteRpsResponse dadosRetorno = new Components.HCorumbaMS.RecepcionarLoteRpsResponse();
                                dadosEnvio.EnviarLoteRpsEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.HCorumbaMS.NfseWSService)servicoWS).RecepcionarLoteRps(dadosEnvio);
                                strRetorno = dadosRetorno.EnviarLoteRpsResposta;
                            }
                            break;
                        #endregion

                        #region Recepcionar Lote RPS - Síncrono
                        case Servicos.NFSeRecepcionarLoteRpsSincrono:
                            if (Empresas.Configuracoes[emp].AmbienteCodigo == (int)TipoAmbiente.taProducao)
                            {
                                ((Components.PCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.PCorumbaMS.RecepcionarLoteRpsSincrono dadosEnvio = new Components.PCorumbaMS.RecepcionarLoteRpsSincrono();
                                Components.PCorumbaMS.RecepcionarLoteRpsSincronoResponse dadosRetorno = new Components.PCorumbaMS.RecepcionarLoteRpsSincronoResponse();
                                dadosEnvio.EnviarLoteRpsSincronoEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.PCorumbaMS.NfseWSService)servicoWS).RecepcionarLoteRpsSincrono(dadosEnvio);
                                strRetorno = dadosRetorno.EnviarLoteRpsSincronoResposta;
                            }
                            else
                            {
                                ((Components.HCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.HCorumbaMS.RecepcionarLoteRpsSincrono dadosEnvio = new Components.HCorumbaMS.RecepcionarLoteRpsSincrono();
                                Components.HCorumbaMS.RecepcionarLoteRpsSincronoResponse dadosRetorno = new Components.HCorumbaMS.RecepcionarLoteRpsSincronoResponse();
                                dadosEnvio.EnviarLoteRpsSincronoEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.HCorumbaMS.NfseWSService)servicoWS).RecepcionarLoteRpsSincrono(dadosEnvio);
                                strRetorno = dadosRetorno.EnviarLoteRpsSincronoResposta;
                            }
                            break;
                        #endregion

                        #region Cancelar RPS
                        case Servicos.NFSeCancelar:
                            if (Empresas.Configuracoes[emp].AmbienteCodigo == (int)TipoAmbiente.taProducao)
                            {
                                ((Components.PCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.PCorumbaMS.CancelarNfse dadosEnvio = new Components.PCorumbaMS.CancelarNfse();
                                Components.PCorumbaMS.CancelarNfseResponse dadosRetorno = new Components.PCorumbaMS.CancelarNfseResponse();
                                dadosEnvio.CancelarNfseEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.PCorumbaMS.NfseWSService)servicoWS).CancelarNfse(dadosEnvio);
                                strRetorno = dadosRetorno.CancelarNfseResposta;
                            }
                            else
                            {
                                ((Components.HCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.HCorumbaMS.CancelarNfse dadosEnvio = new Components.HCorumbaMS.CancelarNfse();
                                Components.HCorumbaMS.CancelarNfseResponse dadosRetorno = new Components.HCorumbaMS.CancelarNfseResponse();
                                dadosEnvio.CancelarNfseEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.HCorumbaMS.NfseWSService)servicoWS).CancelarNfse(dadosEnvio);
                                strRetorno = dadosRetorno.CancelarNfseResposta;
                            }
                            break;
                        #endregion

                        #region Consultar Lote RPS
                        case Servicos.NFSeConsultarLoteRps:
                            if (Empresas.Configuracoes[emp].AmbienteCodigo == (int)TipoAmbiente.taProducao)
                            {
                                ((Components.PCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.PCorumbaMS.ConsultarLoteRps dadosEnvio = new Components.PCorumbaMS.ConsultarLoteRps();
                                Components.PCorumbaMS.ConsultarLoteRpsResponse dadosRetorno = new Components.PCorumbaMS.ConsultarLoteRpsResponse();
                                dadosEnvio.ConsultarLoteRpsEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.PCorumbaMS.NfseWSService)servicoWS).ConsultarLoteRps(dadosEnvio);
                                strRetorno = dadosRetorno.ConsultarLoteRpsResposta;
                            }
                            else
                            {
                                ((Components.HCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.HCorumbaMS.ConsultarLoteRps dadosEnvio = new Components.HCorumbaMS.ConsultarLoteRps();
                                Components.HCorumbaMS.ConsultarLoteRpsResponse dadosRetorno = new Components.HCorumbaMS.ConsultarLoteRpsResponse();
                                dadosEnvio.ConsultarLoteRpsEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.HCorumbaMS.NfseWSService)servicoWS).ConsultarLoteRps(dadosEnvio);
                                strRetorno = dadosRetorno.ConsultarLoteRpsResposta;
                            }
                            break;
                        #endregion

                        #region Consulta Situação Nfse
                        case Servicos.NFSeConsultar:
                            if (Empresas.Configuracoes[emp].AmbienteCodigo == (int)TipoAmbiente.taProducao)
                            {
                                ((Components.PCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.PCorumbaMS.ConsultarNfsePorFaixa dadosEnvio = new Components.PCorumbaMS.ConsultarNfsePorFaixa();
                                Components.PCorumbaMS.ConsultarNfsePorFaixaResponse dadosRetorno = new Components.PCorumbaMS.ConsultarNfsePorFaixaResponse();
                                dadosEnvio.ConsultarNfsePorFaixaEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.PCorumbaMS.NfseWSService)servicoWS).ConsultarNfsePorFaixa(dadosEnvio);
                                strRetorno = dadosRetorno.ConsultarNfsePorFaixaResposta;
                            }
                            else
                            {
                                ((Components.HCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.HCorumbaMS.ConsultarNfsePorFaixa dadosEnvio = new Components.HCorumbaMS.ConsultarNfsePorFaixa();
                                Components.HCorumbaMS.ConsultarNfsePorFaixaResponse dadosRetorno = new Components.HCorumbaMS.ConsultarNfsePorFaixaResponse();
                                dadosEnvio.ConsultarNfsePorFaixaEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.HCorumbaMS.NfseWSService)servicoWS).ConsultarNfsePorFaixa(dadosEnvio);
                                strRetorno = dadosRetorno.ConsultarNfsePorFaixaResposta;
                            }
                            break;
                        #endregion

                        #region Consulta Situação Nfse por RPS
                        case Servicos.NFSeConsultarPorRps:
                            if (Empresas.Configuracoes[emp].AmbienteCodigo == (int)TipoAmbiente.taProducao)
                            {
                                ((Components.PCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.PCorumbaMS.ConsultarNfsePorRps dadosEnvio = new Components.PCorumbaMS.ConsultarNfsePorRps();
                                Components.PCorumbaMS.ConsultarNfsePorRpsResponse dadosRetorno = new Components.PCorumbaMS.ConsultarNfsePorRpsResponse();
                                dadosEnvio.ConsultarNfsePorRpsEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.PCorumbaMS.NfseWSService)servicoWS).ConsultarNfsePorRps(dadosEnvio);
                                strRetorno = dadosRetorno.ConsultarNfsePorRpsResposta;
                            }
                            else
                            {
                                ((Components.HCorumbaMS.NfseWSService)servicoWS).ClientCertificates.Add(Empresas.Configuracoes[emp].X509Certificado);
                                Components.HCorumbaMS.ConsultarNfsePorRps dadosEnvio = new Components.HCorumbaMS.ConsultarNfsePorRps();
                                Components.HCorumbaMS.ConsultarNfsePorRpsResponse dadosRetorno = new Components.HCorumbaMS.ConsultarNfsePorRpsResponse();
                                dadosEnvio.ConsultarNfsePorRpsEnvio = docXML.OuterXml.ToString();
                                dadosRetorno = ((Components.HCorumbaMS.NfseWSService)servicoWS).ConsultarNfsePorRps(dadosEnvio);
                                strRetorno = dadosRetorno.ConsultarNfsePorRpsResposta;
                            }
                            break;
                            #endregion
                    }
                    break;
                #endregion



                #region Demais padrões
                default:
                    if (string.IsNullOrEmpty(cabecMsg))
                        strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { docXML.OuterXml });
                    else
                        strRetorno = wsProxy.InvokeStr(servicoWS, metodo, new object[] { cabecMsg.ToString(), docXML.OuterXml });
                    break;
                    #endregion
            }
            #region gerar arquivos assinados(somente debug)
#if DEBUG
            string path = Application.StartupPath + "\\teste_assintura\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            StreamWriter sw = new StreamWriter(path + "nfseMsg_assinado.xml", true);
            sw.Write(docXML.OuterXml);
            sw.Close();

            StreamWriter sw2 = new StreamWriter(path + "cabecMsg_assinado.xml", true);
            sw2.Write(cabecMsg.ToString());
            sw2.Close();
#endif
            #endregion

            //Atualizar o atributo do serviço da Nfe com o conteúdo retornado do webservice do sefaz                  
            typeServicoNFe.InvokeMember("vStrXmlRetorno", System.Reflection.BindingFlags.SetProperty, null, servicoNFe, new object[] { strRetorno });

            // Registra o retorno de acordo com o status obtido
            if (finalArqEnvio != string.Empty && finalArqRetorno != string.Empty)
            {
                typeServicoNFe.InvokeMember("XmlRetorno", System.Reflection.BindingFlags.InvokeMethod, null, servicoNFe, new Object[] { finalArqEnvio + ".xml", finalArqRetorno + ".xml" });
            }
        }
        #endregion

        #endregion
    }
}

namespace NFe.Exceptions
{
    /// <summary>
    /// Classe para tratamento de exceções da classe Invocar Objeto
    /// </summary>
    public class ExceptionSemInternet : Exception
    {
        public ErroPadrao ErrorCode { get; private set; }

        /// <summary>
        /// Construtor que já define uma mensagem pré-definida de exceção
        /// </summary>
        /// <param name="CodigoErro">Código da mensagem de erro (Classe MsgErro)</param>
        /// <by>Wandrey Mundin Ferreira</by>
        /// <date>24/11/2009</date>
        public ExceptionSemInternet(ErroPadrao Erro)
            : base(MsgErro.ErroPreDefinido(Erro))
        {
            this.ErrorCode = Erro;
        }

        /// <summary>
        /// Construtor que já define uma mensagem pré-definida de exceção com possibilidade de complemento da mensagem
        /// </summary>
        /// <param name="CodigoErro">Código da mensagem de erro (Classe MsgErro)</param>
        /// <param name="ComplementoMensagem">Complemento da mensagem de exceção</param>
        public ExceptionSemInternet(ErroPadrao Erro, string ComplementoMensagem)
            : base(MsgErro.ErroPreDefinido(Erro, ComplementoMensagem))
        {
            this.ErrorCode = Erro;
        }
    }

    /// <summary>
    /// Classe para tratamento de exceções da classe Invocar Objeto, mas exatamente no ponto em que vai enviar o XML para o SEFAZ
    /// </summary>
    public class ExceptionEnvioXML : Exception
    {
        public ErroPadrao ErrorCode { get; private set; }

        /// <summary>
        /// Construtor que já define uma mensagem pré-definida de exceção
        /// </summary>
        /// <param name="CodigoErro">Código da mensagem de erro (Classe MsgErro)</param>
        /// <remarks>
        /// Autor: Wandrey Mundin Ferreira
        /// Data: 16/03/2010
        /// </remarks>
        public ExceptionEnvioXML(ErroPadrao Erro)
            : base(MsgErro.ErroPreDefinido(Erro))
        {
            this.ErrorCode = Erro;
        }

        /// <summary>
        /// Construtor que ´já define uma mensagem pré-definida de exceção com possibilidade de complemento da mensagem
        /// </summary>
        /// <param name="CodigoErro">Código da mensagem de erro (Classe MsgErro)</param>
        /// <param name="ComplementoMensagem">Complemento da mensagem de exceção</param>
        /// <remarks>
        /// Autor: Wandrey Mundin Ferreira
        /// Data: 16/03/2010
        /// </remarks>
        public ExceptionEnvioXML(ErroPadrao Erro, string ComplementoMensagem)
            : base(MsgErro.ErroPreDefinido(Erro, ComplementoMensagem))
        {
            this.ErrorCode = Erro;
        }
    }
}
