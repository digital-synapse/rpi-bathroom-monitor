# rpi-bathroom-monitor

#### Check bathroom status (available/occupied) via android or iOS

This repo contains 2 projects one is a stand alone webserver that runs on a raspberry pi as a startup service. The other is a xamarin client. The stand alone web server monitors a reed switch connected to the GPIO and also exposes an http service that allow the android/ios client to view the bathroom status.

#### Service API methods 

###### GET /bathroom/enqueue/{userid}  
* Arguments: userid (required)  
* Response : 200 (OK)
###### GET /bathroom/status  
* Arguments: none   
* Response : 200 (OK)
```
{
  "Occupied": "true",
  "OccupiedOn": "5/25/2018 5:34:02 PM",
  "OccupieBy": "4746a321-178e-40b1-aa0b-6b6ffc7e7516",
  "Queue": [
    "2c9d9870-9075-4ed2-a8a3-55f729d7c1b8",
    "99ff7b45-b867-4905-8032-f443bd4d01c7"
  ]
}
```
