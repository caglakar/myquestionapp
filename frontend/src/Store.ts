import { act } from 'react-dom/test-utils';
import { couldStartTrivia } from 'typescript';
import { Question } from './Question';
import { QuestionData } from './QuestionData';
import { Store, createStore, combineReducers } from 'redux';

interface QuestionState {
  readonly loading: boolean;
  readonly unanswered: QuestionData[];
  readonly viewing: QuestionData | null;
  readonly searched: QuestionData[];
}

export interface AppState {
  readonly questions: QuestionState;
}

const initialQuestionState: QuestionState = {
  loading: false,
  unanswered: [],
  searched: [],
  viewing: null,
};

export const GETINGUNANSWEREDQUESTIONS = 'GettingUnansweredQuestions';
export const gettingUnansweredQuestionsAction = () =>
  ({ type: GETINGUNANSWEREDQUESTIONS } as const);

export const GOTUNANSWEREDQUESTIONS = 'GotUnAnsweredQuestions';
export const gotUnAnsweredQuestionsAction = (questions: QuestionData[]) =>
  ({ type: GOTUNANSWEREDQUESTIONS, questions: questions } as const);

export const GETINGQUESTION = 'GettingQuestion';
export const gettingQuestionAction = () => ({ type: GETINGQUESTION } as const);

export const GOTQUESTION = 'GotQuestion';
export const gotQuestionAction = (question: QuestionData | null) =>
  ({ type: GOTQUESTION, question: question } as const);

export const SEARCHINGQUESTIONS = 'SearchingQuestions';
export const searchingQuestionAction = () =>
  ({ type: SEARCHINGQUESTIONS } as const);

export const SEARCHEDQUESTIONS = 'SearchedQuestions';
export const searchedQuestionAction = (questions: QuestionData[]) =>
  ({ type: SEARCHEDQUESTIONS, questions } as const);

type QuestionActions =
  | ReturnType<typeof gettingUnansweredQuestionsAction>
  | ReturnType<typeof gotUnAnsweredQuestionsAction>
  | ReturnType<typeof gettingQuestionAction>
  | ReturnType<typeof gotQuestionAction>
  | ReturnType<typeof searchingQuestionAction>
  | ReturnType<typeof searchedQuestionAction>;

const questionReducer = (
  state = initialQuestionState,
  action: QuestionActions,
) => {
  switch (action.type) {
    case GETINGUNANSWEREDQUESTIONS: {
      return {
        ...state,
        loading: true,
      };
    }
    case GOTUNANSWEREDQUESTIONS: {
      return {
        ...state,
        unanswered: action.questions,
        loading: false,
      };
    }
    case GETINGQUESTION: {
      return {
        ...state,
        viewing: null,
        loading: true,
      };
    }
    case GOTQUESTION: {
      return {
        ...state,
        viewing: action.question,
        loading: false,
      };
    }
    case SEARCHINGQUESTIONS: {
      return {
        ...state,
        searched: [],
        loading: true,
      };
    }
    case SEARCHEDQUESTIONS: {
      return {
        ...state,
        searched: action.questions,
        loading: false,
      };
    }
  }
  return state;
};

const rootReducer = combineReducers<AppState>({ questions: questionReducer });

export function configureStore(): Store<AppState> {
  const store = createStore(rootReducer, undefined);
  return store;
}
