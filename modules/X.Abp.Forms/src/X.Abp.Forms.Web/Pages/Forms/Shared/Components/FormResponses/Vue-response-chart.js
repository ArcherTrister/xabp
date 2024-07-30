Vue.component('response-chart', {
    template: `
    <div>
        <div class="form-group mb-3">
            <label>{{question.title}}</label>
            <small class="form-text text-muted">{{responseLength}} Responses</small>
        </div>
  	    <canvas v-bind:id="idData"></canvas>
  	</div>
  `,
    props: ['question', 'type', 'responses', 'options'],
    data() {
        return {
            idData: `chart_${this.question.id}`,
            choices: [],
            answers: [],
            responseLength: 0
        }
    },
    mounted() {
        this.question.choices = _.sortBy(this.question.choices, 'index');
        this.choices = this.question.choices.map(({id, value}) => {
                return {id, value};
            }
        );
        this._chart = new Chart(this.$el.querySelector(`#chart_${this.question.id}`), {
            type: this.type,
            // data: _.clone(this.data),
            options: this.options,
            data: {
                labels: this.question.choices.map(({value}) => value) || [],
                dataRaw: this.question.choices,
                datasets: [
                    {
                        label: "# counts",
                        data: [],
                        backgroundColor: [
                            'rgba(255, 99, 132, 0.2)',
                            'rgba(54, 162, 235, 0.2)',
                            'rgba(255, 206, 86, 0.2)',
                            'rgba(75, 192, 192, 0.2)',
                            'rgba(153, 102, 255, 0.2)',
                            'rgba(255, 159, 64, 0.2)'
                        ],
                        borderColor: [
                            'rgba(255,99,132,1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)',
                            'rgba(75, 192, 192, 1)',
                            'rgba(153, 102, 255, 1)',
                            'rgba(255, 159, 64, 1)'
                        ],
                        borderWidth: 1
                    }
                ]
            }
        });
        this._chart.options.tooltips = {
            callbacks: {
                // this callback is used to create the tooltip label
                label: function (tooltipItem, data) {
                    // get the data label and data value to display
                    // convert the data value to local string so it uses a comma seperated number
                    let dataLabel = data.labels[tooltipItem.index];
                    let value = ': ' + data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toLocaleString();

                    if (dataLabel === 'Other...\n') {
                        const choice = data.dataRaw.find(q => q.value === dataLabel);
                        const otherAnswers = data.answers.filter(q => q.choiceId === choice.id);
                        value += ' (';
                        otherAnswers.forEach((ans, index) => {
                            if (index === 0) {
                                value += ans.value;
                            } else {
                                value += ',' + ans.value;
                            }

                        })
                        value += ')';
                    }

                    // make this isn't a multi-line label (e.g. [["label 1 - line 1, "line 2, ], [etc...]])
                    if (Chart.helpers.isArray(dataLabel)) {
                        // show value on first line of multiline label
                        // need to clone because we are changing the value
                        dataLabel = dataLabel.slice();
                        dataLabel[0] += value;
                    } else {
                        dataLabel += value;
                    }
                    let sum = data.datasets[0].data.reduce((a, b) => a + b, 0);
                    const dataVal = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                    let percentage = parseFloat(((dataVal / sum) * 100).toString()).toFixed(2) + '%';
                    dataLabel += ` (${percentage})`;
                    // return the text to display on the tooltip
                    return dataLabel;
                }
            }
        };
        this.initialize(this.responses);
    },
    methods: {
        groupBy(list, keyGetter) {
            const map = new Map();
            list.forEach((item) => {
                const key = keyGetter(item);
                const collection = map.get(key);
                if (!collection) {
                    map.set(key, [item]);
                } else {
                    collection.push(item);
                }
            });
            return map;
        },
        initialize(data) {
            this.answers = [];
            let answerArray = []
            data.forEach(rp => {
                answerArray.push(rp.answers);
            });
            answerArray = answerArray.flat().filter(q => q.questionId === this.question.id);
            const groupedByResponses = this.groupBy(answerArray, r => r.formResponseId);
            this.responseLength = groupedByResponses.size;
            const idList = answerArray.map(({choiceId}) => choiceId);
            const answerList = answerArray.map(({choiceId, value}) => {
                    return {choiceId, value};
                }
            );
            let result = _.countBy(idList);
            for (let i = 0; i < this.choices.length; i++) {
                this.answers[i] = result[this.choices[i].id] || 0;
            }
            this._chart.data.answers = answerList;
            this._chart.data.datasets[0].data = this.answers;
            this._chart.update();
        }
    },
    watch: {
        'responses': {
            handler(data, oldData) {
                this.initialize(data);
            },
            deep: true
        }
    }
});