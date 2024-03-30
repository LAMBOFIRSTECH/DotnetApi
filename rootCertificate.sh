CANAME=LFT-RootCA

sudo openssl genrsa -aes256 -out $CANAME.key 4096

echo "La clé a bien été créée"

sudo openssl req -x509 -new -nodes -key $CANAME.key -sha256 -days 1826 -out $CANAME.crt

echo "le crt a bien été créée"
