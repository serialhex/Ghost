private void AddCustomer_Click(object sender, System.EventArgs e)
		{
			//step1: verify that Name is not empty
			String name = CustName.Text.Trim() ;
			if( name.Length == 0 )
			{
				MessageBox.Show("Please enter a value for Name.", "Input Validation");
				return;
			} 

			//step2: create the qbXML request
			XmlDocument inputXMLDoc = new XmlDocument();
			inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0",null, null));
			inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"2.0\""));
			XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
			inputXMLDoc.AppendChild(qbXML);
			XmlElement qbXMLMsgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
			XmlElement custAddRq = inputXMLDoc.CreateElement("CustomerAddRq");
			qbXMLMsgsRq.AppendChild(custAddRq);
			custAddRq.SetAttribute("requestID", "1");
			XmlElement custAdd = inputXMLDoc.CreateElement("CustomerAdd");
			custAddRq.AppendChild(custAdd);	
			custAdd.AppendChild(inputXMLDoc.CreateElement("Name")).InnerText=name;
			if( Phone.Text.Length  > 0  )
			{
				custAdd.AppendChild(inputXMLDoc.CreateElement("Phone")).InnerText=Phone.Text;
			}
	
			string input = inputXMLDoc.OuterXml;
			//step3: do the qbXMLRP request
			RequestProcessor2 rp = null; 
			string ticket = null;
			string response = null;
			try 
			{
				rp = new RequestProcessor2 ();
				rp.OpenConnection("", "IDN CustomerAdd C# sample" );
				ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare );
				response = rp.ProcessRequest(ticket, input);
					
			}
			catch( System.Runtime.InteropServices.COMException ex )
			{
				MessageBox.Show( "COM Error Description = " +  ex.Message, "COM error" );
				return;
			}
			finally
			{
				if( ticket != null )
				{
					rp.EndSession(ticket);
				}
				if( rp != null )
				{
					rp.CloseConnection();
				}
			};

			//step4: parse the XML response and show a message
			XmlDocument outputXMLDoc = new XmlDocument();
			outputXMLDoc.LoadXml(response);
			XmlNodeList qbXMLMsgsRsNodeList = outputXMLDoc.GetElementsByTagName ("CustomerAddRs");
			
			if( qbXMLMsgsRsNodeList.Count == 1 ) //it's always true, since we added a single Customer
			{
				System.Text.StringBuilder popupMessage = new System.Text.StringBuilder();

				XmlAttributeCollection rsAttributes = qbXMLMsgsRsNodeList.Item(0).Attributes;
				//get the status Code, info and Severity
				string retStatusCode = rsAttributes.GetNamedItem("statusCode").Value; 
				string retStatusSeverity = rsAttributes.GetNamedItem("statusSeverity").Value;
				string retStatusMessage = rsAttributes.GetNamedItem("statusMessage").Value;
                popupMessage.AppendFormat( "statusCode = {0}, statusSeverity = {1}, statusMessage = {2}",
					retStatusCode, retStatusSeverity, retStatusMessage );

				//get the CustomerRet node for detailed info
				
				//a CustomerAddRs contains max one childNode for "CustomerRet"
				XmlNodeList custAddRsNodeList = qbXMLMsgsRsNodeList.Item(0).ChildNodes;
				if( custAddRsNodeList.Count == 1 && custAddRsNodeList.Item(0).Name.Equals("CustomerRet") )
				{
					XmlNodeList custRetNodeList = custAddRsNodeList.Item(0).ChildNodes ;
				
					foreach (XmlNode custRetNode in custRetNodeList )
					{
						if ( custRetNode.Name.Equals( "ListID" ) )
						{
							popupMessage.AppendFormat("\r\nCustomer ListID = {0}", custRetNode.InnerText  );
						}
						else if ( custRetNode.Name.Equals( "Name" ) )
						{
							popupMessage.AppendFormat("\r\nCustomer Name = {0}", custRetNode.InnerText );
						}
						else if  ( custRetNode.Name.Equals( "FullName" ) )
						{
							popupMessage.AppendFormat("\r\nCustomer FullName = {0}", custRetNode.InnerText ); 
						}
					}
				} // End of customerRet

				MessageBox.Show (popupMessage.ToString(), "QuickBooks response");
			} //End of customerAddRs
		}