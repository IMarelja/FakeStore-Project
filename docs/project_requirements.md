# DESCRIPTION OF THE PROJECT TASK FROM THE OF INFORMATION SYSTEM INTEROPERABILITY

According to the given topic (REST API URL) for the project task implement a "backend" and "frontend" system that implements the following functionalities:

1. A REST API interface that includes a service (endpoint) that will be called by the POST method and send an XML and JSON file. The XML and JSON files must contain arbitrary data for the entity that is bound to the domain of the default REST API interface. The default entity must first be validated, check whether all default data are correct using XSD and JSON schema file validation, and only then save it to the system database. In case of errors, it is necessary to display validation errors to the user. (LO2 – 2 points, LO3 – 2 points, LO5 – 2 points)

2. A SOAP interface that includes a service that receives a term by which to search for an entity. Before that, an XML file must be generated on the "backend" containing data retrieved from one of the REST API methods according to the given topic. The entered term, which is the input data of the SOAP method, must be used to filter only those records that match the given term with the help of XPath and the prepared XML file, and return them as a result of the SOAP method call. (LO2 – 4 points, LO3 – 2 points, LO5 – 4 points)

3. Using Jakarta XML, check the prepared file from the previous step to see if it complies with the validation rules set and return validation messages if the data on the XML file is not valid. (LO2 – 4 points, LO5 – 2 points; LO7 – 2 points)

4. Create an gRPC server application that, using DHMZ (https://vrijeme.hr/hrvatska_n.xml), will enable retrieving the current temperature according to the given city name or part of the city name. If there are multiple entries that match part of the city name, they should all be printed. The service must be available from the client desktop or web application. (LO2 – 4 points, LO3 – 2 points, LO5 – 2 points)

5. Use the REST API and integrate your application with it. Implement an custom version of this REST API interface that connects to the application database and provides all four endpoints (GET, POST, PUT, and DELETE) with JWT tokens (access and refresh) and uses GraphQL. Add a "switch" to the configuration of the application that will allow to change the from the public to the custom REST API interface (LO3 – 8 points, LO4 – 12 points, LO5 – 12 points, LO6 – 2 points, LO7 – 8 points).

6. Write a client desktop or web application (Java or C#) that will contain a graphical interface and enable users to call the service from the first six steps. The application must provide two user roles: read-only (can call only "GET" endpoints) and "full access" (can call all endpoints). (LO1 – 2 points, LO3 – 4 points, LO7 4 points)
