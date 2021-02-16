# (Pol) Plate Recognition
Console App created in MS Visual Studio 2017 enables one to read registracion number from graphic file.
## Table of contents
* [Technologies](#Technologies)
* [Algorithm](#Alorithm)
* [Status of project](#Status_of_project)


## Technologies
Project is created with:
* C# (.Net Framework 4.7.2)
* EmguCV (ver. 4.4.0.4061)
```
PM> Install-Package Emgu.CV -Version 4.4.0.4061
```

## Algorithm
### Detect a plate
![Original photo](https://db3pap004files.storage.live.com/y4mVxxlUvMe_mzDHJA2Q67V5DG1SFVa7tlEnXZjLUKho2lqe6ZgV6A2IoycvfpPdDUO4GBLpmcW2_Apnf8VEsYeq8kLgARRqlzZkKInnY3KAWAh7BITvdG-gBgOaax44T2_GQ_Kb8F-7pEy01TH2Wz6ycLknvRyloWZdj20yTIoXSPeYS2h3kSvFvit7UXScTyI?width=807&height=486&cropmode=none "Original photo")
Original photo

![Photo with blue mask](https://db3pap004files.storage.live.com/y4mWqvNvZNT47uu8IWbtRYuW2Gfpuz_PQUpSXB3huwpQleRspVrjlYh0cil4uFBVYmD_I-yidOsgFne5F7ZN03KvgM_LOJbJZ0IdOR6Vje1L2gc21xNbX3FiHD_FpEHf2Nh5GhhNCP-cH3kAeTiKEd4HG1Q7PittpOhIzDFHqXph0TwcguwpRDP_Cdi7ZxLBpnJ?width=811&height=487&cropmode=none "Photo with blue mask")

![Photo with white mask](https://db3pap004files.storage.live.com/y4mQ8XOoORQ7l9b1FypPIZP2_7rfUdO-A_dMifcImCda8_GtfY2hM-gd2RUzXKVZ6NnwWfJ3Q6iDxzgfKxc03ugVs-lXlG2c-ccwRcETTmpuKLVmh9G7i1T074Zg6iax4bhI3L94yQTEFObrcbzRIMcwJVv8sAc5pznJrDHTEB744fGCJ2A9JHK63yScG8puWkV?width=816&height=486&cropmode=none "Photo with white mask")

![Connected areas](https://db3pap004files.storage.live.com/y4mJpbJUWjUvlRRtvATQQe6xGOWBkiaLTw9aatHvWe70z5R3mJdFmnB35ZTo8DFPhnsiHLh7Lv_HBQFzMW_-gCXJyOt47q18NG3jzJJ2mzutwQk6bsmNk8DnMBA9anBLj6XNN564Q1Yu33heeRIXAhWuLilQryOSgVd-gDyon8rHwdBs9b5BvT1Tklg1PrvPJjY?width=1021&height=610&cropmode=none "Connected areas")

![Seeking corners](https://db3pap004files.storage.live.com/y4m_Sc0r5aOtEV-gsEXTcfweEErdEBdxVgwgcH_Nqpr03Fqb8zKnpltLbdrIN9wagC-ZZVwYk1ekWIWG1zMu_oikR_EGZBqsbM9ujsgGFPsRdIOrCGeBKVF3g7nGnX1bfHMPIkgOVmNrNsBtuwTOID4py0_mIPWLza_5moZkwN9Q2SXjqBhXPLtvPxt_pL70Fab?width=809&height=486&cropmode=none "Seeking corners")
### Abstract a plate
![Plate](https://db3pap004files.storage.live.com/y4m25Nj-oeOkGE9vabSLu2kzoUtsCOCtUW_naLcaSVBq69mmHOcroYEKI-iYe01VHalKp_GEv-3YEconegCGUEsp3jOeiys27brBonMKHhL04_fxImv8w2v6N_3fp31hfbzj7k4ieBlQ8JiQ59Ns0bga7f8yXiUwwydngnWahi0OipXVJBH8UxHULvkxATEDarO?width=505&height=637&cropmode=none "Plate")

### Reading a registration number


## Status of project
In progress.
#### To-Do List:
* detect a plate in vertical picture
* optimalization

