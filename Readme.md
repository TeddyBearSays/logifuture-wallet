In this sample App I used Inmemory DB instead of SQL server just to simplify testing
In real world app it should be SQL server with several replicas

Service doesn't have Auth and permissions validation logic to simplify logic

Service doesn't have Exception handing/Exception conversion Middleware (to provide user friendly error codes instead of 500 errors)



There are several rest endpoints to communicate with service

To not add Users controller with Crud operations nonexisting User created during Wallet creation


Create Wallet:
POST
/api/Wallet - 
[userId] - string($uuid) User Id(will be created if empty)
[currecny] - Wallet currency 


Get Wallet
GET
/api/Wallet/{userId}/{walletId} 


Get All User Wallets
GET
/api/Wallet/{userId}


Add/Remove funds to wallet
POST
/api/Wallet/{walletId}/chage-funds
[Operation] Add/Remove
[Amount] Amount
[Timestamp] DateTime of the operation


Get All Wallet transactions
GET
"{userId}/{walletId}/transactions


Freeze Wallet
POST
"{userId}/{walletId}/transactions


Also there CorrelationIdMiddleware
to request Third party service CorrelationId/TransactionId from 3-party system


All the endpoints are accessible via swagger 

Project can be run via docker:
`docker build -t wallet-api -f Dockerfile .`

Auto service scaling can performed via Kubernetes claster configuration  (3-5 instances with 70% cpu)

kubectl apply -f api-deployment.yaml
kubectl apply -f api-deployment.yaml
kubectl apply -f hpa.yaml