import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { ClassService } from '../../core/services/class.service';
import { AssignmentService } from '../../core/services/assignment.service';
import { GradeService } from '../../core/services/grade.service';
import { StudentService } from '../../core/services/student.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-teacher-dashboard',
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule]
})
export class TeacherDashboardComponent implements OnInit {
  teacherName: string = '';
  activeTab: string = 'classes';
  
  classes: any[] = [];
  classForm: FormGroup;
  classModal: any;
  
  selectedClassId: string = '';
  selectedAssignmentId: string = '';
  students: any[] = [];
  assignments: any[] = [];
  selectedAssignment: any;
  grades: any[] = [];
  
  newAssignment = {
    title: '',
    type: 'Quiz',
    date: new Date().toISOString().split('T')[0],
    maxPoints: 100
  };

  constructor(
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    private classService: ClassService,
    private gradeService: GradeService,
    private studentService: StudentService,
    private assignmentService: AssignmentService
  ) {
    console.log('TeacherDashboardComponent initialized');
    this.classForm = this.fb.group({
      name: ['', Validators.required],
      subject: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    console.log('TeacherDashboardComponent ngOnInit');
    const currentUser = this.authService.currentUserValue;
    if (currentUser) {
      console.log('Current user found:', currentUser.email);
      this.teacherName = `${currentUser.firstName} ${currentUser.lastName}`;
      this.loadClasses();
    } else {
      console.warn('No current user found in TeacherDashboardComponent');
    }
  }

  setActiveTab(tab: string): void {
    console.log(`Changing active tab to: ${tab}`);
    this.activeTab = tab;
  }

  loadClasses(): void {
    console.log('Loading teacher classes...');
    this.classService.getTeacherClasses().subscribe({
      next: (data) => {
        console.log(`Classes loaded successfully: ${data.length} classes found`);
        this.classes = data;
      },
      error: (error) => {
        console.error('Error loading classes:', error);
        if (error.status) {
          console.error(`Status code: ${error.status}, Message: ${error.message}`);
        }
        if (error.error) {
          console.error('Error details:', error.error);
        }
      }
    });
  }

  openCreateClassModal(): void {
    console.log('Opening create class modal');
    this.classForm.reset();
    
    try {
      import('bootstrap').then(({ Modal }) => {
        console.log('Bootstrap loaded successfully');
        if (!document.getElementById('createClassModal')) {
          console.error('Modal element not found in DOM');
          return;
        }
        
        try {
          this.classModal = new Modal(document.getElementById('createClassModal')!);
          console.log('Modal instance created successfully');
          this.classModal.show();
        } catch (err) {
          console.error('Error creating or showing modal:', err);
          console.error('This may be due to the missing @popperjs/core dependency');
        }
      }).catch(err => {
        console.error('Failed to load Bootstrap module:', err);
      });
    } catch (err) {
      console.error('Error in openCreateClassModal:', err);
    }
  }

  createClass(): void {
    console.log('Creating class...');
    if (this.classForm.valid) {
      const classData = this.classForm.value;
      console.log('Class data to submit:', classData);
      
      this.classService.createClass(classData).subscribe({
        next: (response) => {
          console.log('Class created successfully:', response);
          try {
            this.classModal.hide();
          } catch (err) {
            console.error('Error hiding modal:', err);
          }
          this.loadClasses();
        },
        error: (error) => {
          console.error('Error creating class:', error);
          if (error.status) {
            console.error(`Status code: ${error.status}, Message: ${error.message}`);
          }
          if (error.error) {
            console.error('Error details:', error.error);
          }
        }
      });
    } else {
      console.warn('Form is invalid:', this.classForm.errors);
      Object.keys(this.classForm.controls).forEach(key => {
        const control = this.classForm.get(key);
        console.warn(`Control ${key} validity:`, control?.valid, 'Errors:', control?.errors);
      });
    }
  }

  viewClass(classId: string): void {
    console.log(`Navigating to class view: ${classId}`);
    this.router.navigate(['/teacher/classes', classId]);
  }

  editClass(classId: string): void {
    console.log(`Navigating to class edit: ${classId}`);
    this.router.navigate(['/teacher/classes', classId, 'edit']);
  }

  loadStudents(): void {
    if (!this.selectedClassId) {
      console.warn('Cannot load students: No class selected');
      return;
    }
    
    console.log(`Loading students for class: ${this.selectedClassId}`);
    this.studentService.getStudentsByClass(this.selectedClassId).subscribe({
      next: (data) => {
        console.log(`Students loaded successfully: ${data.length} students found`);
        this.students = data;
        this.grades = this.students.map(student => ({
          studentId: student.id,
          score: 0,
          saved: false
        }));
        
        this.loadAssignments();
      },
      error: (error) => {
        console.error('Error loading students:', error);
        if (error.status) {
          console.error(`Status code: ${error.status}, Message: ${error.message}`);
        }
        if (error.error) {
          console.error('Error details:', error.error);
        }
      }
    });
  }

  loadAssignments(): void {
    if (!this.selectedClassId) {
      console.warn('Cannot load assignments: No class selected');
      return;
    }
    
    console.log(`Loading assignments for class: ${this.selectedClassId}`);
    this.assignmentService.getAssignmentsByClass(this.selectedClassId).subscribe({
      next: (data) => {
        console.log(`Assignments loaded successfully: ${data.length} assignments found`);
        this.assignments = data;
      },
      error: (error) => {
        console.error('Error loading assignments:', error);
        if (error.status) {
          console.error(`Status code: ${error.status}, Message: ${error.message}`);
        }
        if (error.error) {
          console.error('Error details:', error.error);
        }
      }
    });
  }

  createAssignment(): void {
    if (!this.selectedClassId) {
      console.warn('Cannot create assignment: No class selected');
      return;
    }
    
    if (!this.newAssignment.title) {
      console.warn('Cannot create assignment: Title is empty');
      return;
    }
    
    console.log(`Creating assignment for class: ${this.selectedClassId}`);
    console.log('Assignment data:', this.newAssignment);
    
    const assignment = {
      ...this.newAssignment,
      classId: this.selectedClassId
    };
    
    this.assignmentService.createAssignment(assignment).subscribe({
      next: (data) => {
        console.log('Assignment created successfully:', data);
        this.loadAssignments();
        this.newAssignment = {
          title: '',
          type: 'Quiz',
          date: new Date().toISOString().split('T')[0],
          maxPoints: 100
        };
      },
      error: (error) => {
        console.error('Error creating assignment:', error);
        if (error.status) {
          console.error(`Status code: ${error.status}, Message: ${error.message}`);
        }
        if (error.error) {
          console.error('Error details:', error.error);
        }
      }
    });
  }

  loadGrades(): void {
    if (!this.selectedAssignmentId || !this.selectedClassId) {
      console.warn('Cannot load grades: No assignment or class selected');
      return;
    }
    
    console.log(`Loading grades for assignment: ${this.selectedAssignmentId}`);
    this.assignmentService.getAssignment(this.selectedAssignmentId).subscribe({
      next: (data) => {
        console.log('Assignment details loaded:', data);
        this.selectedAssignment = data;
        
        this.gradeService.getGradesByAssignment(this.selectedAssignmentId).subscribe({
          next: (gradeData) => {
            console.log(`Grades loaded successfully: ${gradeData.length} grade entries found`);
            this.grades = this.students.map(student => {
              const existingGrade = gradeData.find(g => g.studentId === student.id);
              return {
                studentId: student.id,
                score: existingGrade ? existingGrade.score : 0,
                saved: !!existingGrade,
                gradeId: existingGrade ? existingGrade.id : null
              };
            });
            console.log('Processed grades:', this.grades);
          },
          error: (error) => {
            console.error('Error loading grades:', error);
            if (error.status) {
              console.error(`Status code: ${error.status}, Message: ${error.message}`);
            }
            if (error.error) {
              console.error('Error details:', error.error);
            }
          }
        });
      },
      error: (error) => {
        console.error('Error loading assignment details:', error);
        if (error.status) {
          console.error(`Status code: ${error.status}, Message: ${error.message}`);
        }
        if (error.error) {
          console.error('Error details:', error.error);
        }
      }
    });
  }

  saveGrade(studentId: string, index: number): void {
    console.log(`Saving grade for student ${studentId}, index ${index}`);
    const gradeData = {
      studentId: studentId,
      assignmentId: this.selectedAssignmentId,
      score: this.grades[index].score
    };
    
    console.log('Grade data to save:', gradeData);
    
    if (this.grades[index].gradeId) {
      console.log(`Updating existing grade with ID: ${this.grades[index].gradeId}`);
      this.gradeService.updateGrade(this.grades[index].gradeId, gradeData).subscribe({
        next: (response) => {
          console.log('Grade updated successfully:', response);
          this.grades[index].saved = true;
        },
        error: (error) => {
          console.error('Error updating grade:', error);
          if (error.status) {
            console.error(`Status code: ${error.status}, Message: ${error.message}`);
          }
          if (error.error) {
            console.error('Error details:', error.error);
          }
        }
      });
    } else {
      console.log('Creating new grade entry');
      this.gradeService.createGrade(gradeData).subscribe({
        next: (data) => {
          console.log('Grade created successfully:', data);
          this.grades[index].gradeId = data.id;
          this.grades[index].saved = true;
        },
        error: (error) => {
          console.error('Error saving grade:', error);
          if (error.status) {
            console.error(`Status code: ${error.status}, Message: ${error.message}`);
          }
          if (error.error) {
            console.error('Error details:', error.error);
          }
        }
      });
    }
  }

  saveAllGrades(): void {
    console.log('Saving all unsaved grades');
    const unsavedCount = this.grades.filter(g => !g.saved).length;
    console.log(`Found ${unsavedCount} unsaved grades`);
    
    this.grades.forEach((grade, index) => {
      if (!grade.saved) {
        this.saveGrade(grade.studentId, index);
      }
    });
  }

  logout(): void {
    console.log('Logging out...');
    this.authService.logout().subscribe({
      next: () => {
        console.log('Logout successful, redirecting to login page');
        this.router.navigate(['/auth/login']);
      },
      error: error => {
        console.error('Logout error:', error);
        if (error.status) {
          console.error(`Status code: ${error.status}, Message: ${error.message}`);
        }
        if (error.error) {
          console.error('Error details:', error.error);
        }
      }
    });
  }
}