package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
	"sync"
)

var (
	// Канал для строк
	strChan chan string
	// Канал для обработанных данных
	dataChan chan *Data
)

// Data структура для хранения обработанных данных
type Data struct {
	ID    uint32
	Value float32
}

func main() {

	strChan = make(chan string, 1)
	dataChan = make(chan *Data, 1)
	fileName := "data.csv"

	var wgWorker sync.WaitGroup
	var wgSave sync.WaitGroup

	// Чтение файла
	wgWorker.Add(1)
	go LoadCSV(fileName, strChan, &wgWorker)

	// Запускаем пул воркеров
	for i := 0; i < 5; i++ {
		wgWorker.Add(1)
		go worker(strChan, dataChan, &wgWorker)
	}

	// Сохранение в БД
	wgSave.Add(1)
	go SaveDB(dataChan, &wgSave)

	wgWorker.Wait()
	close(dataChan)

	wgSave.Wait()
}

// LoadCSV чтение файла.
func LoadCSV(fileName string, out chan string, wg *sync.WaitGroup) {
	defer wg.Done()
	file, err := os.Open(fileName)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		out <- scanner.Text()
	}

	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}

	close(out)
}

func worker(in chan string, out chan *Data, wg *sync.WaitGroup) {
	defer wg.Done()
	for s:=range in {

		a := strings.Split(s, ",")
		if len(a) < 2 {
			continue
		}
		id, err := strconv.ParseUint(a[0], 10, 64)
		if err != nil {
			continue
		}
		v, err := strconv.ParseFloat(a[1], 32)
		if err != nil {
			continue
		}

		// Обработка данных
		v = v * 1.1
		d := Data{
			ID:    uint32(id),
			Value: float32(v),
		}
		out <- &d
	}
}

// SaveDB сохранение в БД (заглушка)
func SaveDB(in chan *Data, wg *sync.WaitGroup) {
	defer wg.Done()
	for d:=range in{
		id := d.ID
		v := d.Value

		fmt.Printf("id = %v value = %v\n", id, v)
	}
}
