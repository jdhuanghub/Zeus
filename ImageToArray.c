/****************************************
* Convert a image to c array
* 1. Load a bmp image
* 2. Convert to hexdecimal format data
* 3. Save the array as an c source file
*****************************************/
#include <stdio.h>

int main(int argc, char *argv[])
{
	if (argc == 1)
 	{
 		printf("Error\n Usage: ImageToCArray -<filename>\n");
		return -1;
 	}
	const int READ_SIZE = 40960;
	FILE *binfp;
	FILE *cfp;
	unsigned char buffer[READ_SIZE];
	const char *inputfilename = argv[1];
	int Byte_read = 0;

	//
	//
	binfp = fopen(inputfilename,"rb");
	cfp = fopen("convertedArray.c","wt");
	fprintf(cfp,"//----------------------------------------------\n");
	fprintf(cfp,"//\t\tBMP Image as C Array\n");
	fprintf(cfp,"//----------------------------------------------\n");
	fprintf(cfp,"//This File is created automaticly \n unsigned char NAME[] = \n{\n");

	do 
	{
		Byte_read = fread(buffer,1,READ_SIZE,binfp);
		for (int i = 0; i< Byte_read;i++)
		{
			if (buffer[i] <= 0xf)
			{
				fprintf(cfp,"0x%x ,",buffer[i]);
			}
			else
			{
				fprintf(cfp,"0x%x ,",buffer[i]);
			}
			if (i%16 == 15)
			{
				fprintf(cfp,"\n\t");
			}
		}
	} while (Byte_read == READ_SIZE);
	fprintf(cfp,"\n};\n//end of file");
	fclose(binfp);
	fclose(cfp);
	return 0;
}