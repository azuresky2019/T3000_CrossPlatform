#include "timerx.h"
#include "led.h"
#include "spi.h"

extern u32 	comm_heartbeat;
extern u8 flag_ISP;
extern u8 LED_status[67];
//extern u8 flag_Start;
extern u8  flag_Comm;
extern u16 count_comm;

extern u16 test[200];
extern u8 Mini_Type;

//��ʱ��3�жϷ������	 
void TIM3_IRQHandler(void)
{ 		    		  			    
//	if(TIM_GetFlagStatus(TIM3, TIM_IT_Update) == SET)
//	{
//		LED1 = !LED1;
//	}
//	FGM3_ISR();
	TIM_ClearFlag(TIM3, TIM_IT_Update);	
}

//ͨ�ö�ʱ��3�жϳ�ʼ��
//����ʱ��ѡ��ΪAPB1��2������APB1Ϊ36M
//arr���Զ���װֵ��
//psc��ʱ��Ԥ��Ƶ��
//����ʹ�õ��Ƕ�ʱ��3!
void TIM3_Int_Init(u16 arr, u16 psc)
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3, ENABLE);
	
	TIM_TimeBaseStructure.TIM_Period = arr;
	TIM_TimeBaseStructure.TIM_Prescaler = psc; 
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1; 
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure);
	
	//Timer3 NVIC ����
    NVIC_InitStructure.NVIC_IRQChannel = TIM3_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;	//��ռ���ȼ�0
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;			//�����ȼ�1
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;				//IRQͨ��ʹ��
	NVIC_Init(&NVIC_InitStructure);								//����ָ���Ĳ�����ʼ��NVIC�Ĵ���
	
	TIM_ITConfig(TIM3, TIM_IT_Update, ENABLE);
	TIM_Cmd(TIM3, DISABLE);
}
 	 

/////////////////////////////////////////////////////////////////////////////////////////
u8 DelayTime = 0;
extern u8 flag_led;
void TIM6_IRQHandler(void)
{
	static u16 count1 = 0; // 1s
	if(TIM_GetFlagStatus(TIM6, TIM_IT_Update) == SET)
	{		
		if(flag_led == 1)
			Refresh_LED();  // 500us
	}
	
	comm_heartbeat++;

	if(flag_ISP == 1)
	{  
		// dont receive data from bottom board		
	  if(count1 < ( 150000/ 500))  // 150MS
		{
			count1++;
		}
		else
		{	
			DelayTime++;
			count1 = 0;
		}
		if(DelayTime > 12) 	{ DelayTime = 0; }
		switch(DelayTime)
		{
			case 0:		
				if(Mini_Type == BIG)		LED_status[66] = 5;
				else if(Mini_Type == SMALL)		LED_status[15] = 5;
				else if(Mini_Type == TINY)		LED_status[12] = 5;
				break;
			case 1:		
				if(Mini_Type == BIG)		LED_status[66] = 0 ; 
				else if(Mini_Type == SMALL)		LED_status[15] = 0;
				else if(Mini_Type == TINY)		LED_status[12] = 0;
				break;
			case 2:		
				if(Mini_Type == BIG)		LED_status[66] = 5; 
				else if(Mini_Type == SMALL)		LED_status[15] = 5;
				else if(Mini_Type == TINY)		LED_status[12] = 5;
				break;
			case 3:		
				if(Mini_Type == BIG)		LED_status[66] = 0;
				else if(Mini_Type == SMALL)		LED_status[15] = 0;
				else if(Mini_Type == TINY)		LED_status[12] = 0;
				break;			
			case 12:			DelayTime = 0;	break;			
			default:				 
			break;		
		}
	}
	else if(flag_ISP == 2)
	{
		if(flag_Comm)  // heart beat
		{
			count_comm++;
			if(count_comm > (10000000/ 500))    // 10s
			{
				flag_Comm = 0;
			}		
		}
		if(flag_Comm)
		{
			if(count1 < (500000/ 500))  // 500MS	
			{
				count1++;
			}
			else  
			{
				if(Mini_Type == BIG)		
				{
					if(LED_status[66] == 5)
						LED_status[66] = 0;
					else
						LED_status[66] = 5;
				}
				else if(Mini_Type == SMALL)	
				{
					if(LED_status[15] == 5)
						LED_status[15] = 0;
					else
						LED_status[15] = 5;
				}
				else if(Mini_Type == TINY)	
				{
					if(LED_status[12] == 5)
						LED_status[12] = 0;
					else
						LED_status[12] = 5;
				}				
				count1 = 0;
			}
		}
		else 
		{
			if(count1 < ( 150000/ 500))  
			{
				count1++;
			}
			else
			{	
				DelayTime++;
				count1 = 0;
			}
			switch(DelayTime)
			{
				case 0:	
				case 2:	
				case 4:		
					if(Mini_Type == BIG)		LED_status[66] = 5;
					else if(Mini_Type == SMALL)		LED_status[15] = 5;
					else if(Mini_Type == TINY)		LED_status[12] = 5;
					break;
				case 1:	
				case 3:
				case 5:	
					if(Mini_Type == BIG)		LED_status[66] = 0;
					else if(Mini_Type == SMALL)		LED_status[15] = 0;
					else if(Mini_Type == TINY)		LED_status[12] = 0;
					break;
				case 15:
					DelayTime = 0;	break;
				default:
				break;		
			}
		}
	}	
	
	TIM_ClearFlag(TIM6, TIM_IT_Update);		
}


//������ʱ��6�жϳ�ʼ��					  
//arr���Զ���װֵ��		
//psc��ʱ��Ԥ��Ƶ��		 
//Tout= ((arr+1)*(psc+1))/Tclk��
void TIM6_Int_Init(u16 arr, u16 psc)
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM6, ENABLE);
	
	TIM_TimeBaseStructure.TIM_Period = arr;
	TIM_TimeBaseStructure.TIM_Prescaler = psc; 
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1; 
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseInit(TIM6, &TIM_TimeBaseStructure);
	
	//Timer3 NVIC ����
    NVIC_InitStructure.NVIC_IRQChannel = TIM6_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;	//��ռ���ȼ�3
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 3;			//�����ȼ�3
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;				//IRQͨ��ʹ��
	NVIC_Init(&NVIC_InitStructure);								//����ָ���Ĳ�����ʼ��NVIC�Ĵ���
	
	TIM_ITConfig(TIM6, TIM_IT_Update, ENABLE);
	TIM_Cmd(TIM6, ENABLE);
	
//	RCC->APB1ENR |= 1 << 4;					//TIM6ʱ��ʹ��    
// 	TIM6->ARR = arr;  						//�趨�������Զ���װֵ 
//	TIM6->PSC = psc;  			 			//����Ԥ��Ƶ��.
// 	TIM6->DIER |= 1 << 0;   				//��������ж�				
// 	TIM6->CR1 |= 0x01;    					//ʹ�ܶ�ʱ��6
//	MY_NVIC_Init(0, 0, TIM6_IRQn, 2);		//��ռ1�������ȼ�2����2		
}
