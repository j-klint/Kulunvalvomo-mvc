pushd Harjoittelu
rd /S /Q bin obj
dotnet publish -r linux-x64
pushd bin\Release\net8.0\linux-x64
scp -r publish admon@10.0.0.11:~
popd
popd
ssh admon@10.0.0.11
