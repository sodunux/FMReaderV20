//对内配置
//<<< Use Configuration Wizard   in Context Menu >>>
// <h> LPCD配置
//     <e0>    是否开启AUX通道
//	   </e>	

//     <o1.0..2> 恒流源充电电流选项（根据开发手册，不可修改） 
//									<0=>50nA(Default) 
//									<1=>100nA
//									<2=>150nA
//			     					<3=>200nA
//			     					<4=>250nA
//			     					<5=>300nA
//			     					<6=>350nA
//			     					<7=>400nA



//     <o2.0..3> 中断等待超时配置（根据开发手册，不可修改） 
//									<0=>1ms
//									<1=>2ms
//			     					<2=>3ms
//			     					<3=>4ms
//			     					<4=>5ms
//			     					<5=>6ms
//			     					<6=>7ms
//			     					<7=>8ms
//			     					<8=>9ms
//			     					<9=>10ms(Default) 
//			     					<10=>11ms
//			     					<11=>12ms
//			     					<12=>13ms
//			     					<13=>14ms
//			     					<14=>15ms
//			     					<15=>16ms

// </h>

// <h> 环振调教配置

//     <o3.0..4> 环振调教阈值  （根据开发手册，不可修改）
//									<10=>5.9us
//									<11=>6.5us 
//									<12=>7.1us
//									<13=>7.6us
//									<14=>8.2us
//									<15=>8.8us
//									<16=>9.4us
//									<17=>10us(Default) 
//									<18=>10.6us
//			     					<19=>11.2us
//			     					<20=>11.7us
//			     					<21=>12.3us
// </h>

// <h> LPCD调教配置
//     <o4.0..3> LPCD初始化调教中心点微调范围 （会在定时调教中微调）		
//									<1=>1
//									<2=>2
//									<3=>3
//									<4=>4
//									<5=>5(Default)
//									<6=>6
//									<7=>7
//									<8=>8
//									<9=>9
//									<10=>10
//     <e5>    LPCD调教放大倍数不够是否增大发射功率
//	   </e>	
// </h>


// <h> LPCD检测参数配置

//     <o6.0..1> LPCD采样频率设定 （会在定时调教中微调）		
//									<0=>4分频
//									<1=>8分频(Default)	
//									<2=>16分频 



//     <o7.0..1> LPCD模式选择
//									<0x0=>模式0(Default)
//									<0x1=>模式1
//									<0x2=>模式2

//     <o8.0..1> LPCD模式进入，晶振电流模式选择
//									<0=>大电流模式(Default)
//									<1=>小电流模式

//     <o9.0..1> LPCD模式退出，晶振电流模式选择
//									<0=>大电流模式(Default)
//									<1=>小电流模式



//     <o10.0..3> LPCD误触发噪声和温漂判断范围
//									<1=>1
//									<2=>2
//									<3=>3
//									<4=>4
//									<5=>5(Default)
//									<6=>6
//									<7=>7
//									<8=>8
//									<9=>9
//									<10=>10
// </h>

//1 3 8 9 10 11 19 20

#define  	LPCD_AUX_EN	    			1   		//0
#define  	CHARGE_CURRENT   			0x00   		//1
#define	 	IRQ_TIMEOUT					9			//2
#define	 	LP10K_CALIBRA_THRESHOLD 	17			//3
#define		PULSE_CENTER_RANGE			5			//4
#define  	LPCD_CALIBRA_REDUCE_PMOS	17			//5
#define	 	LPCD_PULSE_SAMPLE_CLK  		1			//6
#define		LPCD_MODE					0x0			//7
#define		LP_OSC_MODE					0			//8
#define		WUP_OSC_MODE				0			//9
#define		ERR_TRIG_JUDGE_RANGE		5			//10
//<<< end of configuration section >>>


#define		LP10K_CNT_DEFAULT			84	//用1.69Mhz对10K的高电平进行采样，0x54是缺省值

