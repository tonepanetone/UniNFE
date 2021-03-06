﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

using NFe.Components;
using NFe.Settings;
using NFe.Certificado;
using NFe.Exceptions;

namespace NFe.Service
{
    public class TaskDFeRecepcao : TaskAbst
    {
        public override void Execute()
        {
            int emp = Empresas.FindEmpresaByThread();
            distDFeInt _distDFeInt = new distDFeInt();

            Servico = Servicos.DFeEnviar;
            try
            {
                if (!this.vXmlNfeDadosMsgEhXML)
                {
                    ///versao|1.00
                    ///tpAmb|1
                    ///cUFAutor|35
                    ///CNPJ|
                    /// ou
                    ///CPF|
                    ///ultNSU|123456789012345
                    /// ou
                    ///NSU|123456789012345
                    List<string> cLinhas = Functions.LerArquivo(this.NomeArquivoXML);
                    Functions.PopulateClasse(_distDFeInt, cLinhas);

                    string f = System.IO.Path.GetFileNameWithoutExtension(NomeArquivoXML) + ".xml";

                    if (NomeArquivoXML.IndexOf(Empresas.Configuracoes[emp].PastaValidar, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        f = Path.Combine(Empresas.Configuracoes[emp].PastaValidar, f);
                    }
                    // Gerar o XML de envio de DFe a partir do TXT gerado pelo ERP
                    oGerarXML.RecepcaoDFe(f, _distDFeInt);
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(this.NomeArquivoXML);

                    XmlNodeList consdistDFeIntList = doc.GetElementsByTagName("distDFeInt");

                    foreach (XmlNode consdistDFeIntNode in consdistDFeIntList)
                    {
                        XmlElement consdistDFeIntElemento = (XmlElement)consdistDFeIntNode;
                        Functions.PopulateClasse(_distDFeInt, consdistDFeIntElemento);
                    }

                    //Definir o objeto do WebService
                    WebServiceProxy wsProxy =
                        ConfiguracaoApp.DefinirWS(Servico,
                                                    emp,
                                                    991,
                                                    _distDFeInt.tpAmb);
                    System.Net.SecurityProtocolType securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(991, _distDFeInt.tpAmb, 1, PadroesNFSe.NaoIdentificado, Servico);

                    object oConsNFDestEvento = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);

                    //Invocar o método que envia o XML para o SEFAZ
                    oInvocarObj.Invocar(wsProxy,
                                        oConsNFDestEvento,
                                        wsProxy.NomeMetodoWS[0],
                                        null,
                                        this,
                                        Propriedade.Extensao(Propriedade.TipoEnvio.EnvDFe).EnvioXML,
                                        Propriedade.Extensao(Propriedade.TipoEnvio.EnvDFe).RetornoXML,
                                        true,
                                        securityProtocolType);

                    LeRetornoDFe(emp, doc);
                }
            }
            catch (Exception ex)
            {
                WriteLogError(ex);
            }
            finally
            {
                try
                {
                    //Deletar o arquivo de solicitação do serviço
                    Functions.DeletarArquivo(NomeArquivoXML);
                }
                catch
                {
                }
            }
        }

        private void LeRetornoDFe(int emp, XmlDocument doc)
        {
            /*
            if (string.IsNullOrEmpty(Empresas.Configuracoes[emp].PastaDownloadNFeDest))
            {
                ///
                /// nao interpreto como erro, já que o ERP pode não querer descompactar os arquivos
                /// 
                Auxiliar.WriteLog("LeRetornoNFe: Pasta de DownloadNFeDest nao definida");
                return;
            }
            */

            try
            {
                ///
                /// pega o nome base dos arquivos a serem gravados
                /// 
                string fileRetorno2 = Functions.ExtrairNomeArq(this.NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.EnvDFe).EnvioXML);
                ///
                /// pega o nome do arquivo de retorno
                /// 
                string fileRetorno = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                  fileRetorno2 + Propriedade.Extensao(Propriedade.TipoEnvio.EnvDFe).RetornoXML);

                if (!File.Exists(fileRetorno))
                {
                    return;
                }
                ///
                /// cria a pasta para comportar as notas e eventos retornados já descompactados
                /// 
                string folderTerceiros = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno, "dfe");
                if (!Directory.Exists(folderTerceiros))
                    Directory.CreateDirectory(folderTerceiros);

                ///
                /// exclui todos os arquivos que foram envolvidos no retorno
                /// 
                foreach (var item in Directory.GetFiles(folderTerceiros, fileRetorno2 + "-*.xml", SearchOption.TopDirectoryOnly))
                    if (!Functions.FileInUse(item))
                        File.Delete(item);

                doc.Load(fileRetorno);
                XmlNodeList envEventoList = doc.GetElementsByTagName("retDistDFeInt");
                foreach (XmlNode ret1Node in envEventoList)
                {
                    XmlElement ret1Elemento = (XmlElement)ret1Node;

                    XmlNodeList ret1List = ret1Elemento.GetElementsByTagName("loteDistDFeInt");
                    foreach (XmlNode ret in ret1List)
                    {
                        for (int n = 0; n < ret.ChildNodes.Count; ++n)
                        {
                            if (ret.ChildNodes[n].Name.Equals("docZip"))
                            {
                                string FileToFtp = "";
                                string NSU = ret.ChildNodes[n].Attributes[TpcnResources.NSU.ToString()].Value;

                                ///
                                /// descompacta o conteudo
                                /// 
                                string xmlRes = TFunctions.Decompress(ret.ChildNodes[n].InnerText);
                                if (string.IsNullOrEmpty(xmlRes))
                                {
                                    Auxiliar.WriteLog("LeRetornoNFe: Não foi possivel descompactar o conteudo da NSU: " + NSU, false);
                                }
                                else
                                {
                                    if (ret.ChildNodes[n].Attributes["schema"].InnerText.StartsWith("resEvento"))
                                    {
                                        FileToFtp = Path.Combine(folderTerceiros, fileRetorno2 + "-" + NSU + Propriedade.Extensao(Propriedade.TipoEnvio.PedEve).RetornoXML);
                                    }
                                    else if (ret.ChildNodes[n].Attributes["schema"].InnerText.StartsWith("procEventoNFe"))
                                    {
                                        FileToFtp = Path.Combine(folderTerceiros, fileRetorno2 + "-" + NSU + Propriedade.ExtRetorno.ProcEventoNFe);
                                    }
                                    else if (ret.ChildNodes[n].Attributes["schema"].InnerText.StartsWith("procNFe"))
                                    {
                                        FileToFtp = Path.Combine(folderTerceiros, fileRetorno2 + "-" + NSU + Propriedade.ExtRetorno.ProcNFe);
                                    }
                                    else if (ret.ChildNodes[n].Attributes["schema"].InnerText.StartsWith("resNFe"))
                                    {
                                        FileToFtp = Path.Combine(folderTerceiros, fileRetorno2 + "-" + NSU + Propriedade.Extensao(Propriedade.TipoEnvio.NFe).EnvioXML);
                                    }
                                    else
                                        Auxiliar.WriteLog("LerRetornoDFe:  Nao foi possivel ler o schema", false);

                                    if (FileToFtp != "")
                                    {
                                        if (!File.Exists(FileToFtp))
                                            File.WriteAllText(FileToFtp, xmlRes);

                                        string vFolder = Empresas.Configuracoes[emp].FTPPastaRetornos;
                                        if (!string.IsNullOrEmpty(vFolder))
                                        {
                                            try
                                            {
                                                Empresas.Configuracoes[emp].SendFileToFTP(FileToFtp, vFolder);
                                            }
                                            catch (Exception ex)
                                            {
                                                ///
                                                /// grava um arquivo de erro com extensao "FTP" para diferenciar dos arquivos de erro
                                                oAux.GravarArqErroERP(Path.ChangeExtension(fileRetorno, ".ftp"), ex.Message);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Auxiliar.WriteLog("LeRetornoNFe: " + ex.Message, false);
                ///
                /// Wandrey.
                /// Foi tudo processado mas houve algum erro na descompactacao dos retornos
                /// Se gravar o arquivo com extensao .err, o ERP pode ignorar o XML de retorno, que está correto
                /// 
                //WriteLogError(ex);
            }
        }

        private void WriteLogError(Exception ex)
        {
            var extRet = vXmlNfeDadosMsgEhXML ? Propriedade.Extensao(Propriedade.TipoEnvio.EnvDFe).EnvioXML : 
                                                Propriedade.Extensao(Propriedade.TipoEnvio.EnvDFe).EnvioTXT;

            try
            {
                //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                TFunctions.GravarArqErroServico(NomeArquivoXML, extRet, Propriedade.ExtRetorno.retEnvDFe_ERR, ex);
            }
            catch
            {
                //Se falhou algo na hora de gravar o retorno .ERR (de erro) para o ERP, infelizmente não posso fazer mais nada.
                //Wandrey 09/03/2010
            }
        }
    }
}
