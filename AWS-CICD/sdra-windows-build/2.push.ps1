$psw=$(aws ecr get-login-password --region us-west-2)
docker login -u AWS -p $psw 624240287035.dkr.ecr.us-west-2.amazonaws.com/sdra-windows-build
docker push 624240287035.dkr.ecr.us-west-2.amazonaws.com/sdra-windows-build:4.8
