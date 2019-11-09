#written by Jon Stone 
#This program was written to read log files while removing unnecessary lines 
#The remaining lines of data are read where data is parsed and then placed into an Excel file for readability.

#words that need to be removed or kept from the log file
remove_word = ['RemoveWord1', 'RemoveWord2']
keep_word = ['KeepWord1', 'KeepWord2']

import os
import re
import glob
import csv
import xlwt
import os
import sys
import xlsxwriter

#path to where the log files are stored
Path = "\\"
filelist = os.listdir(Path)

#this writes all the log files to the console, useful to see at times
#print(glob.glob("\\*.log"))

#loop to remove the unneccessary words and write to a new log file
for i in filelist:
         with open(Path + i, 'r') as oldfile, open('newlog.txt','w+') as newfile:
             for line in oldfile:
                 if not any(remove_word in line for remove_word in remove_word):
                     if any(keep_word in line for keep_word in keep_word):
                         #removes certain words and replaces with a blank space
                         line = line.replace("GET","")
                         line = line.replace("- 80 -","|")
                         line = line.partition("HTTP")[0]
                         #this removes the server IP address
                         line = line[:22] + "" + line[37:]
                         #adds pipe element between date and time
                         line = line[:10] + " | " + line[11:]
                         #this creates an even whitespace
                         line = re.sub(' +', '',line)
                         newfile.write(line + '\n')

#look for input file, path is set to the same as this .py file
inputfilename = os.path.join(os.path.dirname(sys.argv[0]), 'newlog.txt')

#get the path of the input file as the target output path
targetoutputpath = os.path.dirname(inputfilename)

#create the workbook and worksheet
workbook = xlsxwriter.Workbook('logs.xlsx')
worksheet = workbook.add_worksheet()

#adding a bold format to use for the headers
bold = workbook.add_format({'bold': True})

#widen columns
worksheet.set_column('A:A', 12)
worksheet.set_column('B:B', 12)
worksheet.set_column('C:C', 47)
worksheet.set_column('D:D', 18)

#write column headers
worksheet.write('A1','Date', bold)
worksheet.write('B1','Time', bold)
worksheet.write('C1','File', bold)
worksheet.write('D1','IP Address', bold)

#CSV reader object set up for reading the input file with pipe delimiters
datareader = csv.reader(open(inputfilename, 'rt'),
                        delimiter='|', quotechar='"')

#process the file and output to Excel sheet
for rowno, row in enumerate(datareader):
    for colno, colitem in enumerate(row):
        worksheet.write(rowno+1, colno, colitem)

#close the workbook
workbook.close()