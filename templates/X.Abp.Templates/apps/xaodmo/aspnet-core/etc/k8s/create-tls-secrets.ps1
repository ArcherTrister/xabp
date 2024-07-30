mkcert `
"MyProjectName-st-authserver" `
"MyProjectName-st-web" `
"MyProjectName-st-public-web" `
"MyProjectName-st-gateway-web" "MyProjectName-st-gateway-web-public" `
"MyProjectName-st-identity" "MyProjectName-st-administration" "MyProjectName-st-saas" "MyProjectName-st-product" 
kubectl create namespace MyProjectName
kubectl create secret tls -n MyProjectName MyProjectName-tls --cert=./MyProjectName-st-authserver+8.pem  --key=./MyProjectName-st-authserver+8-key.pem
