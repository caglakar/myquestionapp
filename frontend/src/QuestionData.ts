import { http } from './http';
import { getAccessToken } from './Auth';
export interface QuestionData {
  questionId: number;
  title: string;
  content: string;
  userName: string;
  created: Date;
  answers: AnswerData[];
}
export interface AnswerData {
  answerId: number;
  content: string;
  userName: string;
  created: Date;
}

const questions: QuestionData[] = [
  {
    questionId: 1,
    title: 'Why should I learn TypeScript?',
    content:
      'TypeScript seems to be getting popular so I wondered whether it is worth my time learning it? What benefits does it give over JavaScript?',
    userName: 'Bob',
    created: new Date(),
    answers: [
      {
        answerId: 1,
        content: 'To catch problems earlier speeding up your developments',
        userName: 'Jane',
        created: new Date(),
      },
      {
        answerId: 2,
        content:
          'So, that you can use the JavaScript features of tomorrow, today',
        userName: 'Fred',
        created: new Date(),
      },
    ],
  },
  {
    questionId: 2,
    title: 'Which state management tool should I use?',
    content:
      'There seem to be a fair few state management tools around for React - React, Unstated, ... Which one should I use?',
    userName: 'Bob',
    created: new Date(),
    answers: [],
  },
];

export interface QuestionDataFromServer {
  questionId: number;
  title: string;
  content: string;
  userName: string;
  created: string;
  answers: Array<{
    answerId: number;
    content: string;
    userName: string;
    created: string;
  }>;
}

export const mapQuestionFromServer = (
  question: QuestionDataFromServer,
): QuestionData => ({
  ...question,
  created: new Date(question.created),
  answers: question.answers
    ? question.answers.map((answer) => ({
        ...answer,
        created: new Date(answer.created),
      }))
    : [],
});

export const getUnansweredQuestions = async (): Promise<QuestionData[]> => {
  // // await wait(500);
  // // return questions.filter((q) => q.answers.length === 0);
  // let unansweredQuestions: QuestionData[] = [];
  // const response = await fetch(
  //   'http://localhost:37892/api/questions/unanswered',
  // );
  // unansweredQuestions = await response.json();
  // return unansweredQuestions.map((question) => ({
  //   ...question,
  //   created: new Date(question.created),
  // }));

  const result = await http<QuestionDataFromServer[]>({
    path: '/questions/unanswered',
  });
  if (result.ok && result.body) {
    return result.body.map(mapQuestionFromServer);
  } else {
    return [];
  }
};

const wait = async (ms: number): Promise<void> => {
  return new Promise((resolve) => setTimeout(resolve, ms));
};

export const getQuestion = async (
  questionId: number,
): Promise<QuestionData | null> => {
  // await wait(500);
  // const results = questions.filter((q) => q.questionId === questionId);
  // return results.length === 0 ? null : results[0];
  const result = await http<QuestionDataFromServer>({
    path: `/questions/${questionId}`,
  });
  if (result.ok && result.body) {
    return mapQuestionFromServer(result.body);
  } else {
    return null;
  }
};

export const searchQuestions = async (
  criteria: string,
): Promise<QuestionData[]> => {
  // await wait(500);
  // return questions.filter(
  //   (q) =>
  //     q.title.toLowerCase().indexOf(criteria.toLowerCase()) >= 0 ||
  //     q.content.toLowerCase().indexOf(criteria.toLowerCase()) >= 0,
  // );
  const result = await http<QuestionDataFromServer[]>({
    path: `/question?search=${criteria}`,
  });
  if (result.ok && result.body) {
    return result.body.map(mapQuestionFromServer);
  } else {
    return [];
  }
};

export interface PostQuestionData {
  title: string;
  content: string;
  userName: string;
  created: Date;
}

// export const postQuestion = async (
//   question: PostQuestionData,
// ): Promise<QuestionData | undefined> => {
//   await wait(500);
//   const questionId = Math.max(...questions.map((q) => q.questionId)) + 1;
//   const newQuestion: QuestionData = { ...question, questionId, answers: [] };
//   questions.push(newQuestion);
//   return newQuestion;
// };

export const postQuestion = async (
  question: PostQuestionData,
): Promise<QuestionData | undefined> => {
  const accessToken = await getAccessToken();
  const result = await http<QuestionDataFromServer, PostQuestionData>({
    path: '/questions',
    method: 'post',
    body: question,
    accessToken,
  });
  if (result.ok && result.body) {
    return mapQuestionFromServer(result.body);
  } else {
    return undefined;
  }
};

export interface PostAnswerData {
  questionId: number;
  content: string;
  userName: string;
  created: Date;
}

// export const postAnswer = async (
//   answer: PostAnswerData,
// ): Promise<AnswerData | undefined> => {
//   await wait(500);
//   const question = questions.filter(
//     (q) => q.questionId === answer.questionId,
//   )[0];
//   const answerInQuestion: AnswerData = { answerId: 99, ...answer };
//   question.answers.push(answerInQuestion);
//   return answerInQuestion;
// };
export const postAnswer = async (
  answer: PostAnswerData,
): Promise<AnswerData | undefined> => {
  const accessToken = await getAccessToken();
  const result = await http<AnswerData, PostAnswerData>({
    path: '/questions/answer',
    method: 'post',
    body: answer,
    accessToken,
  });
  if (result.ok) {
    return result.body;
  } else {
    return undefined;
  }
};
