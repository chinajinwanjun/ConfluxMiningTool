#!/bin/sh
walletRow=`cat #dir#/default.toml |grep 'mining_author='`
#wallet=`echo $walletRow| sed -En 's/mining_author="(\w+)"/\1/p'`
wallet=`echo $walletRow| grep -oP '(?<=")(\w+)(?=")'`

#echo $wallet
 
#handle the trust ip list
ipList=`cat #dir#/net_config/trusted_nodes.json |sed -En 's/(\d+)/\1/p'|grep -Eow "[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+"|sed -e :a -e ';$!N;s/\n/,/;ta'`
postData='http://47.91.220.168:7777/home/StoreTrustNode?info='$wallet'MMMMMM'$ipList
echo $postData
wget -O test.html $postData
